<template>
    <div class="pcPhotos-container">
          <el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
            <el-form :model="queryParams" ref="queryForm" :inline="true">
              <el-form-item label="订单号">
                <el-input v-model="queryParams.searchKey" clearable="" placeholder="请输入订单号" />
              </el-form-item>
              <el-form-item label="创建时间">
                <el-date-picker placeholder="请选择创建时间" value-format="YYYY/MM/DD" type="daterange"
                  v-model="queryParams.createTimeRange" />
              </el-form-item>
              <el-form-item>
                <el-button-group>
                  <el-button type="primary" icon="ele-Search" @click="handleQuery" > 查询 </el-button>
                  <el-button icon="ele-Refresh" @click="() => queryParams = {}"> 重置 </el-button>
                  <el-button @click="openAddDialog">新增模拟数据</el-button>
                </el-button-group>  
              </el-form-item>  
            </el-form>
          </el-card>
          <el-card class="full-table" shadow="hover" style="margin-top: 8px;">
            <el-table :data="tableData" style="width: 100%;" v-loading="loading" tooltip-effect="light" row-key="id"
              border="">
              <el-table-column type="index" label="序号" width="55" align="center" />
              <el-table-column prop="outTradeNumber" label="商户订单号" width="180"></el-table-column>
              <el-table-column prop="transactionId" label="支付订单号" width="220"></el-table-column>
              <el-table-column prop="description" label="描述" width="180"></el-table-column>
              <el-table-column prop="total" :formatter="amountFormatter" label="金额" width="70"></el-table-column>
              <el-table-column prop="tradeState" label="状态" width="70">
                <template #default="scope">
                  <el-tag v-if="scope.row.tradeState == 'SUCCESS'" type="success"> 完成 </el-tag>
                  <el-tag v-else-if="scope.row.tradeState == 'REFUND'" type="danger"> 退款 </el-tag>
                  <el-tag v-else type="info"> 未完成 </el-tag>
                </template>
              </el-table-column>
              <el-table-column prop="attachment" label="附加信息" width="180"></el-table-column>
              <el-table-column prop="tags" label="业务类型" width="90"></el-table-column>
              <el-table-column prop="createTime" label="创建时间" width="150"></el-table-column>
              <el-table-column prop="successTime" label="完成时间" width="150"></el-table-column>
              <el-table-column prop="businessId" label="业务ID" width="130"></el-table-column>
              <el-table-column label="操作" align="center" fixed="right">
                <template #default="scope">
                  <el-button size="small" text="" type="primary"
                    v-if="scope.row.qrcodeContent != null && scope.row.qrcodeContent != '' && (scope.row.tradeState === '' || (!scope.row.tradeState))"
                    @click="openQrDialog(scope.row.qrcodeContent)">付款二维码</el-button>
  
                  <el-button size="small" text="" type="primary" v-if="scope.row.tradeState === 'REFUND'"
                    @click="openRefundDialog(scope.row.transactionId)">查看退款</el-button>
  
                  <el-button size="small" text="" type="primary" v-if="scope.row.tradeState === 'SUCCESS'"
                    @click="doRefund(scope.row)">全额退款</el-button>
                </template>
              </el-table-column>
            </el-table>
            <el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
              :total="tableParams.total" :page-sizes="[10, 20, 50, 100, 200]" background="" @size-change="handleSizeChange"
              @current-change="handleCurrentChange" layout="total, sizes, prev, pager, next, jumper" />
        </el-card>
      <el-dialog v-model="showAddDialog" title="新增模拟数据">
        <el-form>
          <el-form-item label="商品">
            <el-input v-model="addData.description" placeholder="必填" clearable />
          </el-form-item>
          <el-form-item label="金额(分)">
            <el-input v-model="addData.total" placeholder="必填，填数字,单位是分" clearable />
          </el-form-item>
          <el-form-item label="附加信息">
            <el-input v-model="addData.attachment" clearable />
          </el-form-item>
        </el-form>
        <template #footer>
          <span class="dialog-footer">
            <el-button @click="closeAddDialog">取 消</el-button>
            <el-button type="primary" @click="saveData">确 定</el-button>
          </span>
        </template>
      </el-dialog>
      <el-dialog title="付款二维码" v-model="showQrDialog">
        <div ref="qrDiv"></div>
      </el-dialog>
      <el-dialog title="退款信息" v-model="showRefundDialog">
        <el-table :data="subTableData" style="width: 100%;" tooltip-effect="light" row-key="id" border="">
          <el-table-column type="index" label="序号" width="55" align="center" />
          <el-table-column prop="outRefundNumber" label="商户退款号" width="180"></el-table-column>
          <el-table-column prop="transactionId" label="支付订单号" width="220"></el-table-column>
          <el-table-column prop="refund" label="金额(分)" width="70"></el-table-column>
          <el-table-column prop="reason" label="退款原因" width="180"></el-table-column>
          <el-table-column prop="tradeState" label="状态" width="70">
            <template #default="scope">
              <el-tag v-if="scope.row.tradeState == 'SUCCESS'" type="success"> 完成 </el-tag>
              <el-tag v-else-if="scope.row.tradeState == 'REFUND'" type="danger"> 退款 </el-tag>
              <el-tag v-else type="info"> 未完成 </el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="remark" label="备注" width="180"></el-table-column>
          <el-table-column prop="createTime" label="创建时间" width="150"></el-table-column>
          <el-table-column prop="successTime" label="完成时间" width="150"></el-table-column>
        </el-table>
      </el-dialog>
    </div>
  </template>
  <script setup lang="ts" name="weChatPay">
  import { ref, nextTick } from "vue";
  import { ElMessageBox, ElMessage } from "element-plus";
  import { auth } from '/@/utils/authFunction';
  import { pagePayList, createPay, getRefundListByID, refundDomestic } from '/@/api/system/weChatPay';
  import QRCode from 'qrcodejs2-fixes';
  
  const qrDiv = ref<HTMLElement | null>(null);
  const qrValue = ref('http://example.com');
  const showAddDialog = ref(false);
  const showQrDialog = ref(false);
  const showRefundDialog = ref(false);
  const loading = ref(false);
  const tableData = ref<any>([]);
  const subTableData = ref<any>([]);
  const addData = ref<any>({});
  const queryParams = ref<any>({});
  const tableParams = ref({
    page: 1,
    pageSize: 10,
    total: 0,
  });
  
  // 查询操作
  const handleQuery = async () => {
    loading.value = true;
    var res = await pagePayList(Object.assign(queryParams.value, tableParams.value));
    let tmpRows = res.data.result?.items ?? [];
    tableData.value = tmpRows;
    tableParams.value.total = res.data.result?.total;
    loading.value = false;
  }
  
  // 改变页面容量
  const handleSizeChange = (val: number) => {
    tableParams.value.pageSize = val;
    handleQuery();
  }
  
  // 改变页码序号
  const handleCurrentChange = (val: number) => {
    tableParams.value.page = val;
    handleQuery();
  }
  
  //退款
  const doRefund = async (orderInfo: any) => {
    ElMessageBox.prompt(`确定进行退款：${orderInfo.total / 100}元?请输入退款理由`, '提示', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
    }).then(async ({ value }) => {
      let resp = await refundDomestic({
        tradeId: orderInfo.outTradeNumber,
        reason: value,
        refund: orderInfo.total,
        total: orderInfo.total
      });
      console.log(resp);
      if (resp.data.code == 200) {
        ElMessage.success(`【${value}】退款申请成功`);
      }
      else {
        ElMessage.error('操作失败：' + resp.data.message);
      }
    }).catch(() => {
      ElMessage.error('取消操作');
    });
  }
  
  const amountFormatter = (row: any, column: any, cellValue: number, index: number) => {
    return (cellValue / 100).toFixed(2);
  }
  const openAddDialog = () => {
    addData.value = {
      description: null,
      total: null,
      attachment: null
    }
    showAddDialog.value = true;
  }
  const closeAddDialog = () => {
    showAddDialog.value = false;
  }
  const openQrDialog = (code: string) => {
    showQrDialog.value = true;
    nextTick(() => {
      (<HTMLElement>qrDiv.value).innerHTML = '';
      new QRCode(qrDiv.value, {
        text: code,
        width: 260,
        height: 260,
        colorDark: '#000000',
        colorLight: '#ffffff',
      });
    });
  }
  const closeQrDialog = () => {
    showQrDialog.value = false;
  }
  const openRefundDialog = async (code: string) => {
    var res = await getRefundListByID(code);
    if (res.data.code === 200) {
      let tmpRows = res.data.result ?? [];
      subTableData.value = tmpRows;
      showRefundDialog.value = true;
    } else {
      ElMessage.error('获取退款列表失败，' + res.data.message);
    }
  }
  const closeRefundDialog = () => {
    showRefundDialog.value = false;
  }
  const saveData = async () => {
    var res = await createPay(addData.value);
    if (res.data.code === 200) {
      closeAddDialog();
      let code = res.data.result.qrcodeUrl;
      openQrDialog(code);
      handleQuery();
    } else {
      ElMessage.error('新建失败，' + res.data.message);
    }
  }
  
  handleQuery();
  </script>
  