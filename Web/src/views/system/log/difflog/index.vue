<template>
	<div class="sys-difflog-container">
		<el-card shadow="hover" class="card-tight">
			<el-form :model="state.queryParams" ref="queryForm" :inline="true">
				<el-form-item label="租户" v-if="userStore.userInfos.accountType == 999">
					<TenantSelect v-model="state.queryParams.tenantId" clearable />
				</el-form-item>
				<el-form-item label="开始时间">
					<el-date-picker v-model="state.queryParams.startTime" type="datetime" placeholder="开始时间" value-format="YYYY-MM-DD HH:mm:ss" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item label="结束时间">
					<el-date-picker v-model="state.queryParams.endTime" type="datetime" placeholder="结束时间" value-format="YYYY-MM-DD HH:mm:ss" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item>
					<el-button-group>
						<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysDifflog:page'"> 查询 </el-button>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					</el-button-group>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card class="full-table" shadow="hover" style="margin-top: 5px">
			<el-table :data="state.logData" style="width: 100%" v-loading="state.loading" border>
        <el-table-column type="expand">
          <template #default="scope">
            <el-card header="差异数据" style="width: 100%; margin: 5px">
              <el-table :data="item.columns" v-for="item in scope.row.diffData" :key="item.tableName" :span-method="(data: any) => diffTableSpanMethod(data, item)" border style="width: 100%">
                <el-table-column label="表名" width="200">
                  <template #default>
                    {{item.tableName}}
                    <br/>
                    {{item.tableDescription}}
                  </template>
                </el-table-column>
                <el-table-column prop="columnName" label="字段描述" width="300" :formatter="(row: any) => `${row.columnName} - ${row.columnDescription}`" />
                <el-table-column prop="beforeValue" label="修改前" show-overflow-tooltip>
                  <template #default="columnScope">
                    <pre v-html="markDiff(columnScope.row.beforeValue, columnScope.row.afterValue, true)" />
                  </template>
                </el-table-column>
                <el-table-column prop="afterValue" label="修改后" show-overflow-tooltip>
                  <template #default="columnScope">
                    <pre v-html="markDiff(columnScope.row.beforeValue, columnScope.row.afterValue, false)" />
                  </template>
                </el-table-column>
              </el-table>
              <el-table :data="[ { sql: scope.row.sql } ]" border style="width: 100%">
                <el-table-column prop="sql" label="SQL语句">
                  <template #default>
                    <pre class="sql" v-html="formatSql(scope.row.sql)"></pre>
                  </template>
                </el-table-column>
              </el-table>
              <el-table :data="scope.row.parameters" border style="width: 100%">
                <el-table-column prop="parameterName" label="参数名" width="200" />
                <el-table-column prop="typeName" label="类型" width="100" />
                <el-table-column prop="value" label="值" />
              </el-table>
            </el-card>
          </template>
        </el-table-column>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="diffType" label="差异操作" header-align="center" show-overflow-tooltip />
				<el-table-column prop="elapsed" label="耗时(ms)" header-align="center" show-overflow-tooltip />
				<el-table-column prop="message" label="日志消息" header-align="center" show-overflow-tooltip />
				<el-table-column prop="businessData" label="业务对象" header-align="center" show-overflow-tooltip />
				<el-table-column prop="createTime" label="操作时间" align="center" show-overflow-tooltip />
			</el-table>
			<el-pagination
				v-model:currentPage="state.tableParams.page"
				v-model:page-size="state.tableParams.pageSize"
				:total="state.tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				size="small"
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>
	</div>
</template>

<script lang="ts" setup name="sysDiffLog">
import { onMounted, reactive } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysLogDiffApi, SysTenantApi } from '/@/api-services/api';
import { SysLogDiff } from '/@/api-services/models';
import { useUserInfo } from "/@/stores/userInfo";
import TenantSelect from '/@/views/system/tenant/component/tenantSelect.vue';

const userStore = useUserInfo();
const state = reactive({
	loading: false,
	queryParams: {
		tenantId: undefined,
		startTime: undefined,
		endTime: undefined,
	},
	tableParams: {
		page: 1,
		pageSize: 50,
		total: 0 as any,
	},
	logData: [] as Array<SysLogDiff>,
});

onMounted(async () => {
	if (userStore.userInfos.accountType == 999) {
		state.queryParams.tenantId = userStore.userInfos.currentTenantId as any;
	}
	handleQuery();
});

// 查询操作
const handleQuery = async () => {
	if (state.queryParams.startTime == null) state.queryParams.startTime = undefined;
	if (state.queryParams.endTime == null) state.queryParams.endTime = undefined;

	state.loading = true;
	let params = Object.assign(state.queryParams, state.tableParams);
	var res = await getAPI(SysLogDiffApi).apiSysLogDiffPagePost(params);
	state.logData = res.data.result?.items ?? [];
  state.logData.forEach(e => {
    e.diffData = JSON.parse(e.diffData ?? "[]");
    e.parameters = JSON.parse(e.parameters ?? "[]");
  });
	state.tableParams.total = res.data.result?.total;
	state.loading = false;
};

// 重置操作
const resetQuery = () => {
	state.queryParams.startTime = undefined;
	state.queryParams.endTime = undefined;
	handleQuery();
};

// 改变页面容量
const handleSizeChange = (val: number) => {
	state.tableParams.pageSize = val;
	handleQuery();
};

// 改变页码序号
const handleCurrentChange = (val: number) => {
	state.tableParams.page = val;
	handleQuery();
};

// 合并差异表格表名列
const diffTableSpanMethod = ({columnIndex, rowIndex}: any, itme: any) => {
  if (columnIndex === 0) {
    if (rowIndex === 0) {
      return {
        rowspan: itme.columns.length,
        colspan: 1
      }
    } else {
      return {
        rowspan: 0,
        colspan: 0
      }
    }
  }
}

const formatSql = (sql: string) => {
  // 移除多余的空格
  let formatted = sql.replace(/\s+/g, ' ').trim();

  // 替换反引号包裹的字段
  formatted = formatted.replace(/`([^`]+)`/g, '<span class="sql-backtick">`$1`</span>');

  // 替换@参数
  formatted = formatted.replace(/(@\w+)/g, '<span class="sql-param">$1</span>');

  // 替换SQL关键字
  formatted = formatted.replace(/\b(INSERT|DELETE|UPDATE|SELECT|FROM|SET|JOIN|ON|AND|OR|IN|NOT|IS|NULL|WHERE|TRUE|FALSE|LIKE|ORDER BY|GROUP BY|HAVING|LIMIT|AS|WITH|CASE|WHEN|THEN|ELSE|END)\b/g, '<span class="sql-keyword">$1</span>');

  // 智能换行
  // 在SET和VALUES后面添加换行
  formatted = formatted.replace(/(SET|VALUES)(?=\s)/g, '$1\n    ');
  // 在逗号后面添加换行，除非是最后一个逗号
  formatted = formatted.replace(/,(?![^]*?,\s*$)(?=[^\s])/g, ',\n    ');
  // 在WHERE前添加换行，如果WHERE前面不是逗号
  formatted = formatted.replace(/([\s\S]+)(WHERE)/g, '$1\n$2');

  // 移除由于换行添加的多余空格
  formatted = formatted.replace(/\n\s*\n/g, '\n');

  return formatted;
};

function lcs(s1: string, s2: string): number[][] {
  const m = s1.length;
  const n = s2.length;
  const dp = Array.from({ length: m + 1 }, () => Array(n + 1).fill(0));

  for (let i = 1; i <= m; i++) {
    for (let j = 1; j <= n; j++) {
      if (s1[i - 1] === s2[j - 1]) {
        dp[i][j] = dp[i - 1][j - 1] + 1;
      } else {
        dp[i][j] = Math.max(dp[i - 1][j], dp[i][j - 1]);
      }
    }
  }
  return dp;
}

function markDiff(oldData: any, newData: any, returnOld: boolean): string {
  if (typeof oldData !== 'string' || typeof newData !== 'string') {
    return `<span class="diff-${returnOld ? 'delete' : 'add'}">${returnOld ? oldData : newData}</span>`;
  }

  const dp = lcs(oldData, newData);
  const m = oldData.length;
  const n = newData.length;
  let oldIndex = m, newIndex = n;
  const diffResult: { type: string, content: string }[] = [];

  while (oldIndex > 0 || newIndex > 0) {
    if (oldIndex > 0 && newIndex > 0 && oldData[oldIndex - 1] === newData[newIndex - 1]) {
      diffResult.push({ type: 'unchanged', content: oldData[oldIndex - 1] });
      oldIndex--;
      newIndex--;
    } else if (newIndex > 0 && (oldIndex === 0 || dp[oldIndex][newIndex - 1] >= dp[oldIndex - 1][newIndex])) {
      diffResult.push({ type: 'add', content: newData[newIndex - 1] });
      newIndex--;
    } else {
      diffResult.push({ type: 'delete', content: oldData[oldIndex - 1] });
      oldIndex--;
    }
  }

  const result = diffResult.reverse().map(chunk => {
    switch (chunk.type) {
      case 'add': return `<span class="diff-add">${chunk.content}</span>`;
      case 'delete': return `<span class="diff-delete">${chunk.content}</span>`;
      default: return chunk.content;
    }
  }).join('');

  return result.replace(returnOld ? /<span class="diff-add">(.*?)<\/span>/g : /<span class="diff-delete">(.*?)<\/span>/g, '');
}

const shortcuts = [
	{
		text: '今天',
		value: new Date(),
	},
	{
		text: '昨天',
		value: () => {
			const date = new Date();
			date.setTime(date.getTime() - 3600 * 1000 * 24);
			return date;
		},
	},
	{
		text: '上周',
		value: () => {
			const date = new Date();
			date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
			return date;
		},
	},
];
</script>

<style lang="scss" scoped>
.el-popper {
	max-width: 60%;
}
:deep(pre.sql) {
  white-space: pre-wrap;
  .sql-param { color: green; }
  .sql-keyword { color: blue; }
  .sql-backtick { color: blueviolet; }
  span.diff-unchanged { color: inherit; }
  span.diff-delete { color: red; }
  span.diff-add { color: green; }
}
:deep(pre) {
  span.diff-delete { color: red; }
  span.diff-add { color: green; }
}
</style>
