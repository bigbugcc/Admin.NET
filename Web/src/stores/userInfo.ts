import { defineStore } from 'pinia';
import { Local, Session } from '/@/utils/storage';
import Watermark from '/@/utils/watermark';
import { useThemeConfig } from '/@/stores/themeConfig';

import { getAPI } from '/@/utils/axios-utils';
import { SysAuthApi, SysConstApi, SysDictTypeApi } from '/@/api-services/api';

/**
 * 用户信息
 * @methods setUserInfos 设置用户信息
 */
export const useUserInfo = defineStore('userInfo', {
	state: (): UserInfosState => ({
		userInfos: {} as any,
		constList: [] as any,
		dictList: {} as any,
		dictListInt: {} as any,
	}),
	getters: {
		// // 获取系统常量列表
		// async getSysConstList(): Promise<any[]> {
		// 	var res = await getAPI(SysConstApi).apiSysConstListGet();
		// 	this.constList = res.data.result ?? [];
		// 	return this.constList;
		// },
	},
	actions: {
		// 存储用户信息到浏览器缓存
		async setUserInfos() {
			if (Session.get('userInfo')) {
				this.userInfos = Session.get('userInfo');
			} else {
				const userInfos = <UserInfos>await this.getApiUserInfo();
				this.userInfos = userInfos;
			}
		},

		// 存储常量信息到浏览器缓存
		async setConstList() {
			if (Session.get('constList')) {
				this.constList = Session.get('constList');
			} else {
				const constList = <any[]>await this.getSysConstList();
				Session.set('constList', constList);
				this.constList = constList;
			}
		},

		// 存储字典信息到浏览器缓存
		async setDictList() {
			var res = await getAPI(SysDictTypeApi).apiSysDictTypeAllDictListGet();
			this.dictList = res.data.result;
			// if (Session.get('dictList')) {
			// 	this.dictList = Session.get('dictList');
			// } else {
			//	const dictList = <any[]>await this.getAllDictList();
			//	Session.set('dictList', dictList);
			//	this.dictList = dictList;
			// }
		},

		// 获取当前用户信息
		getApiUserInfo() {
			return new Promise((resolve) => {
				getAPI(SysAuthApi)
					.apiSysAuthUserInfoGet()
					.then(async (res: any) => {
						if (res.data.result == null) return;
						var d = res.data.result;
						const userInfos = {
							id: d.id,
							account: d.account,
							realName: d.realName,
							phone: d.phone,
							idCardNum: d.idCardNum,
							email: d.email,
							accountType: d.accountType,
							avatar: d.avatar ?? '/favicon.ico',
							address: d.address,
							signature: d.signature,
							orgId: d.orgId,
							orgName: d.orgName,
							posName: d.posName,
							roles: d.roleIds,
							authBtnList: d.buttons,
							time: new Date().getTime(),
						};
						// vue-next-admin 提交Id：225bce7 提交消息：admin-23.03.26:发布v2.4.32版本
						// 增加了下面代码，引起当前会话的用户信息不会刷新，如：重新提交的头像不更新，需要新开一个页面才能正确显示
						// Session.set('userInfo', userInfos);

						// 用户水印
						const storesThemeConfig = useThemeConfig();
						storesThemeConfig.themeConfig.watermarkText = d.watermarkText ?? '';
						if (storesThemeConfig.themeConfig.isWatermark) Watermark.set(storesThemeConfig.themeConfig.watermarkText);
						else Watermark.del();

						Local.remove('themeConfig');
						Local.set('themeConfig', storesThemeConfig.themeConfig);

						resolve(userInfos);
					});
			});
		},

		// 获取常量集合
		getSysConstList() {
			return new Promise((resolve) => {
				getAPI(SysConstApi)
					.apiSysConstListGet()
					.then(async (res: any) => {
						resolve(res.data.result ?? []);
					});
			});
		},

		// 获取字典集合
		getAllDictList() {
			return new Promise((resolve) => {
				if (this.dictList) {
					resolve(this.dictList);
				} else {
					getAPI(SysDictTypeApi)
						.apiSysDictTypeAllDictListGet()
						.then((res: any) => {
							resolve(res.data.result ?? []);
						});
				}
			});
		},

		// 根据字典类型和代码取字典项
		getDictItemByCode(typePCode: string, val: string) {
			if (val) {
				const _val = val.toString();
				const ds = this.getDictDatasByCode(typePCode);
				for (let index = 0; index < ds.length; index++) {
					const element = ds[index];
					if (element.code == _val) {
						return element;
					}
				}
			}
			return {};
		},

		// 根据字典类型和值取描述
		getDictLabelByVal(typePCode: string, val: string) {
			if (val) {
				const _val = val.toString();
				const ds = this.getDictDatasByCode(typePCode);
				for (let index = 0; index < ds.length; index++) {
					const element = ds[index];
					if (element.value == _val) {
						return element;
					}
				}
			}
			return {};
		},

		// 根据字典类型和描述取值
		getDictValByLabel(typePCode: string, label: string) {
			if (!label) return '';
			const ds = this.getDictDatasByCode(typePCode);
			for (let index = 0; index < ds.length; index++) {
				const element = ds[index];
				if (element.name == label) {
					return element;
				}
			}
		},

		// 根据字典类型获取字典数据
		getDictDatasByCode(dictTypeCode: string) {
			return this.dictList[dictTypeCode] || [];
		},

		// 根据字典类型获取字典数据（值转为数字类型）
		getDictIntDatasByCode(dictTypeCode: string) {
			var ds = this.dictListInt[dictTypeCode];
			if (ds) {
				return ds;
			} else {
				ds = this.dictList[dictTypeCode].map((element: { code: any }) => {
					var d = { ...element };
					d.code = element.code - 0;
					return d;
				});
				this.dictListInt[dictTypeCode] = ds;
				return ds;
			}
		},
	},
});
