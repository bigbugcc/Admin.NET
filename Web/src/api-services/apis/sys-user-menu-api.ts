/* tslint:disable */
/* eslint-disable */
/**
 * Admin.NET 通用权限开发平台
 * 让 .NET 开发更简单、更通用、更流行。整合最新技术，模块插件式开发，前后端分离，开箱即用。<br/><u><b><font color='FF0000'> 👮不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！</font></b></u>
 *
 * OpenAPI spec version: 1.0.0
 * 
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */

import globalAxios, { AxiosResponse, AxiosInstance, AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
// Some imports not used depending on template conditions
// @ts-ignore
import { BASE_PATH, COLLECTION_FORMATS, RequestArgs, BaseAPI, RequiredError } from '../base';
import { AdminResultListInt64 } from '../models';
import { AdminResultListMenuOutput } from '../models';
import { UserMenuInput } from '../models';
/**
 * SysUserMenuApi - axios parameter creator
 * @export
 */
export const SysUserMenuApiAxiosParamCreator = function (configuration?: Configuration) {
    return {
        /**
         * 
         * @summary 收藏菜单 🔖
         * @param {UserMenuInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysUserMenuAddPost: async (body?: UserMenuInput, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/api/sysUserMenu/add`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'POST', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required
            // http bearer authentication required
            if (configuration && configuration.accessToken) {
                const accessToken = typeof configuration.accessToken === 'function'
                    ? await configuration.accessToken()
                    : await configuration.accessToken;
                localVarHeaderParameter["Authorization"] = "Bearer " + accessToken;
            }

            localVarHeaderParameter['Content-Type'] = 'application/json-patch+json';

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};
            const needsSerialization = (typeof body !== "string") || localVarRequestOptions.headers['Content-Type'] === 'application/json';
            localVarRequestOptions.data =  needsSerialization ? JSON.stringify(body !== undefined ? body : {}) : (body || "");

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
        /**
         * 
         * @summary 取消收藏菜单 🔖
         * @param {UserMenuInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysUserMenuDeletePost: async (body?: UserMenuInput, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/api/sysUserMenu/delete`;
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'POST', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required
            // http bearer authentication required
            if (configuration && configuration.accessToken) {
                const accessToken = typeof configuration.accessToken === 'function'
                    ? await configuration.accessToken()
                    : await configuration.accessToken;
                localVarHeaderParameter["Authorization"] = "Bearer " + accessToken;
            }

            localVarHeaderParameter['Content-Type'] = 'application/json-patch+json';

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};
            const needsSerialization = (typeof body !== "string") || localVarRequestOptions.headers['Content-Type'] === 'application/json';
            localVarRequestOptions.data =  needsSerialization ? JSON.stringify(body !== undefined ? body : {}) : (body || "");

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
        /**
         * 
         * @summary 根据用户Id删除收藏菜单 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysUserMenuDeleteUserMenuByUserIdUserIdPost: async (userId: number, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            // verify required parameter 'userId' is not null or undefined
            if (userId === null || userId === undefined) {
                throw new RequiredError('userId','Required parameter userId was null or undefined when calling apiSysUserMenuDeleteUserMenuByUserIdUserIdPost.');
            }
            const localVarPath = `/api/sysUserMenu/deleteUserMenuByUserId/{userId}`
                .replace(`{${"userId"}}`, encodeURIComponent(String(userId)));
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'POST', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required
            // http bearer authentication required
            if (configuration && configuration.accessToken) {
                const accessToken = typeof configuration.accessToken === 'function'
                    ? await configuration.accessToken()
                    : await configuration.accessToken;
                localVarHeaderParameter["Authorization"] = "Bearer " + accessToken;
            }

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
        /**
         * 
         * @summary 根据用户Id获取收藏菜单Id集合 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysUserMenuUserMenuIdListUserIdGet: async (userId: number, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            // verify required parameter 'userId' is not null or undefined
            if (userId === null || userId === undefined) {
                throw new RequiredError('userId','Required parameter userId was null or undefined when calling apiSysUserMenuUserMenuIdListUserIdGet.');
            }
            const localVarPath = `/api/sysUserMenu/userMenuIdList/{userId}`
                .replace(`{${"userId"}}`, encodeURIComponent(String(userId)));
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'GET', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required
            // http bearer authentication required
            if (configuration && configuration.accessToken) {
                const accessToken = typeof configuration.accessToken === 'function'
                    ? await configuration.accessToken()
                    : await configuration.accessToken;
                localVarHeaderParameter["Authorization"] = "Bearer " + accessToken;
            }

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
        /**
         * 
         * @summary 根据用户Id获取收藏菜单集合 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysUserMenuUserMenuListUserIdGet: async (userId: number, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            // verify required parameter 'userId' is not null or undefined
            if (userId === null || userId === undefined) {
                throw new RequiredError('userId','Required parameter userId was null or undefined when calling apiSysUserMenuUserMenuListUserIdGet.');
            }
            const localVarPath = `/api/sysUserMenu/userMenuList/{userId}`
                .replace(`{${"userId"}}`, encodeURIComponent(String(userId)));
            // use dummy base URL string because the URL constructor only accepts absolute URLs.
            const localVarUrlObj = new URL(localVarPath, 'https://example.com');
            let baseOptions;
            if (configuration) {
                baseOptions = configuration.baseOptions;
            }
            const localVarRequestOptions :AxiosRequestConfig = { method: 'GET', ...baseOptions, ...options};
            const localVarHeaderParameter = {} as any;
            const localVarQueryParameter = {} as any;

            // authentication Bearer required
            // http bearer authentication required
            if (configuration && configuration.accessToken) {
                const accessToken = typeof configuration.accessToken === 'function'
                    ? await configuration.accessToken()
                    : await configuration.accessToken;
                localVarHeaderParameter["Authorization"] = "Bearer " + accessToken;
            }

            const query = new URLSearchParams(localVarUrlObj.search);
            for (const key in localVarQueryParameter) {
                query.set(key, localVarQueryParameter[key]);
            }
            for (const key in options.params) {
                query.set(key, options.params[key]);
            }
            localVarUrlObj.search = (new URLSearchParams(query)).toString();
            let headersFromBaseOptions = baseOptions && baseOptions.headers ? baseOptions.headers : {};
            localVarRequestOptions.headers = {...localVarHeaderParameter, ...headersFromBaseOptions, ...options.headers};

            return {
                url: localVarUrlObj.pathname + localVarUrlObj.search + localVarUrlObj.hash,
                options: localVarRequestOptions,
            };
        },
    }
};

/**
 * SysUserMenuApi - functional programming interface
 * @export
 */
export const SysUserMenuApiFp = function(configuration?: Configuration) {
    return {
        /**
         * 
         * @summary 收藏菜单 🔖
         * @param {UserMenuInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuAddPost(body?: UserMenuInput, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<void>>> {
            const localVarAxiosArgs = await SysUserMenuApiAxiosParamCreator(configuration).apiSysUserMenuAddPost(body, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 取消收藏菜单 🔖
         * @param {UserMenuInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuDeletePost(body?: UserMenuInput, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<void>>> {
            const localVarAxiosArgs = await SysUserMenuApiAxiosParamCreator(configuration).apiSysUserMenuDeletePost(body, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 根据用户Id删除收藏菜单 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuDeleteUserMenuByUserIdUserIdPost(userId: number, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<void>>> {
            const localVarAxiosArgs = await SysUserMenuApiAxiosParamCreator(configuration).apiSysUserMenuDeleteUserMenuByUserIdUserIdPost(userId, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 根据用户Id获取收藏菜单Id集合 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuUserMenuIdListUserIdGet(userId: number, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<AdminResultListInt64>>> {
            const localVarAxiosArgs = await SysUserMenuApiAxiosParamCreator(configuration).apiSysUserMenuUserMenuIdListUserIdGet(userId, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 根据用户Id获取收藏菜单集合 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuUserMenuListUserIdGet(userId: number, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<AdminResultListMenuOutput>>> {
            const localVarAxiosArgs = await SysUserMenuApiAxiosParamCreator(configuration).apiSysUserMenuUserMenuListUserIdGet(userId, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
    }
};

/**
 * SysUserMenuApi - factory interface
 * @export
 */
export const SysUserMenuApiFactory = function (configuration?: Configuration, basePath?: string, axios?: AxiosInstance) {
    return {
        /**
         * 
         * @summary 收藏菜单 🔖
         * @param {UserMenuInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuAddPost(body?: UserMenuInput, options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
            return SysUserMenuApiFp(configuration).apiSysUserMenuAddPost(body, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 取消收藏菜单 🔖
         * @param {UserMenuInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuDeletePost(body?: UserMenuInput, options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
            return SysUserMenuApiFp(configuration).apiSysUserMenuDeletePost(body, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 根据用户Id删除收藏菜单 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuDeleteUserMenuByUserIdUserIdPost(userId: number, options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
            return SysUserMenuApiFp(configuration).apiSysUserMenuDeleteUserMenuByUserIdUserIdPost(userId, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 根据用户Id获取收藏菜单Id集合 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuUserMenuIdListUserIdGet(userId: number, options?: AxiosRequestConfig): Promise<AxiosResponse<AdminResultListInt64>> {
            return SysUserMenuApiFp(configuration).apiSysUserMenuUserMenuIdListUserIdGet(userId, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 根据用户Id获取收藏菜单集合 🔖
         * @param {number} userId 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysUserMenuUserMenuListUserIdGet(userId: number, options?: AxiosRequestConfig): Promise<AxiosResponse<AdminResultListMenuOutput>> {
            return SysUserMenuApiFp(configuration).apiSysUserMenuUserMenuListUserIdGet(userId, options).then((request) => request(axios, basePath));
        },
    };
};

/**
 * SysUserMenuApi - object-oriented interface
 * @export
 * @class SysUserMenuApi
 * @extends {BaseAPI}
 */
export class SysUserMenuApi extends BaseAPI {
    /**
     * 
     * @summary 收藏菜单 🔖
     * @param {UserMenuInput} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysUserMenuApi
     */
    public async apiSysUserMenuAddPost(body?: UserMenuInput, options?: AxiosRequestConfig) : Promise<AxiosResponse<void>> {
        return SysUserMenuApiFp(this.configuration).apiSysUserMenuAddPost(body, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 取消收藏菜单 🔖
     * @param {UserMenuInput} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysUserMenuApi
     */
    public async apiSysUserMenuDeletePost(body?: UserMenuInput, options?: AxiosRequestConfig) : Promise<AxiosResponse<void>> {
        return SysUserMenuApiFp(this.configuration).apiSysUserMenuDeletePost(body, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 根据用户Id删除收藏菜单 🔖
     * @param {number} userId 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysUserMenuApi
     */
    public async apiSysUserMenuDeleteUserMenuByUserIdUserIdPost(userId: number, options?: AxiosRequestConfig) : Promise<AxiosResponse<void>> {
        return SysUserMenuApiFp(this.configuration).apiSysUserMenuDeleteUserMenuByUserIdUserIdPost(userId, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 根据用户Id获取收藏菜单Id集合 🔖
     * @param {number} userId 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysUserMenuApi
     */
    public async apiSysUserMenuUserMenuIdListUserIdGet(userId: number, options?: AxiosRequestConfig) : Promise<AxiosResponse<AdminResultListInt64>> {
        return SysUserMenuApiFp(this.configuration).apiSysUserMenuUserMenuIdListUserIdGet(userId, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 根据用户Id获取收藏菜单集合 🔖
     * @param {number} userId 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysUserMenuApi
     */
    public async apiSysUserMenuUserMenuListUserIdGet(userId: number, options?: AxiosRequestConfig) : Promise<AxiosResponse<AdminResultListMenuOutput>> {
        return SysUserMenuApiFp(this.configuration).apiSysUserMenuUserMenuListUserIdGet(userId, options).then((request) => request(this.axios, this.basePath));
    }
}
