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
import { AddPluginInput } from '../models';
import { AdminResultSqlSugarPagedListSysPlugin } from '../models';
import { AdminResultString } from '../models';
import { DeletePluginInput } from '../models';
import { PagePluginInput } from '../models';
import { UpdatePluginInput } from '../models';
/**
 * SysPluginApi - axios parameter creator
 * @export
 */
export const SysPluginApiAxiosParamCreator = function (configuration?: Configuration) {
    return {
        /**
         * 
         * @summary 增加动态插件 🧩
         * @param {AddPluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysPluginAddPost: async (body?: AddPluginInput, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/api/sysPlugin/add`;
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
         * @summary 添加动态程序集/接口 🧩
         * @param {string} [body] 
         * @param {string} [assemblyName] 程序集名称
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysPluginCompileAssemblyPost: async (body?: string, assemblyName?: string, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/api/sysPlugin/compileAssembly`;
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

            if (assemblyName !== undefined) {
                localVarQueryParameter['assemblyName'] = assemblyName;
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
         * @summary 删除动态插件 🧩
         * @param {DeletePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysPluginDeletePost: async (body?: DeletePluginInput, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/api/sysPlugin/delete`;
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
         * @summary 获取动态插件列表 🧩
         * @param {PagePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysPluginPagePost: async (body?: PagePluginInput, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/api/sysPlugin/page`;
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
         * @summary 移除动态程序集/接口 🧩
         * @param {string} assemblyName 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysPluginRemoveAssemblyAssemblyNamePost: async (assemblyName: string, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            // verify required parameter 'assemblyName' is not null or undefined
            if (assemblyName === null || assemblyName === undefined) {
                throw new RequiredError('assemblyName','Required parameter assemblyName was null or undefined when calling apiSysPluginRemoveAssemblyAssemblyNamePost.');
            }
            const localVarPath = `/api/sysPlugin/removeAssembly/{assemblyName}`
                .replace(`{${"assemblyName"}}`, encodeURIComponent(String(assemblyName)));
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
         * @summary 更新动态插件 🧩
         * @param {UpdatePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        apiSysPluginUpdatePost: async (body?: UpdatePluginInput, options: AxiosRequestConfig = {}): Promise<RequestArgs> => {
            const localVarPath = `/api/sysPlugin/update`;
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
    }
};

/**
 * SysPluginApi - functional programming interface
 * @export
 */
export const SysPluginApiFp = function(configuration?: Configuration) {
    return {
        /**
         * 
         * @summary 增加动态插件 🧩
         * @param {AddPluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginAddPost(body?: AddPluginInput, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<void>>> {
            const localVarAxiosArgs = await SysPluginApiAxiosParamCreator(configuration).apiSysPluginAddPost(body, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 添加动态程序集/接口 🧩
         * @param {string} [body] 
         * @param {string} [assemblyName] 程序集名称
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginCompileAssemblyPost(body?: string, assemblyName?: string, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<AdminResultString>>> {
            const localVarAxiosArgs = await SysPluginApiAxiosParamCreator(configuration).apiSysPluginCompileAssemblyPost(body, assemblyName, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 删除动态插件 🧩
         * @param {DeletePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginDeletePost(body?: DeletePluginInput, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<void>>> {
            const localVarAxiosArgs = await SysPluginApiAxiosParamCreator(configuration).apiSysPluginDeletePost(body, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 获取动态插件列表 🧩
         * @param {PagePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginPagePost(body?: PagePluginInput, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<AdminResultSqlSugarPagedListSysPlugin>>> {
            const localVarAxiosArgs = await SysPluginApiAxiosParamCreator(configuration).apiSysPluginPagePost(body, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 移除动态程序集/接口 🧩
         * @param {string} assemblyName 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginRemoveAssemblyAssemblyNamePost(assemblyName: string, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<void>>> {
            const localVarAxiosArgs = await SysPluginApiAxiosParamCreator(configuration).apiSysPluginRemoveAssemblyAssemblyNamePost(assemblyName, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
        /**
         * 
         * @summary 更新动态插件 🧩
         * @param {UpdatePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginUpdatePost(body?: UpdatePluginInput, options?: AxiosRequestConfig): Promise<(axios?: AxiosInstance, basePath?: string) => Promise<AxiosResponse<void>>> {
            const localVarAxiosArgs = await SysPluginApiAxiosParamCreator(configuration).apiSysPluginUpdatePost(body, options);
            return (axios: AxiosInstance = globalAxios, basePath: string = BASE_PATH) => {
                const axiosRequestArgs :AxiosRequestConfig = {...localVarAxiosArgs.options, url: basePath + localVarAxiosArgs.url};
                return axios.request(axiosRequestArgs);
            };
        },
    }
};

/**
 * SysPluginApi - factory interface
 * @export
 */
export const SysPluginApiFactory = function (configuration?: Configuration, basePath?: string, axios?: AxiosInstance) {
    return {
        /**
         * 
         * @summary 增加动态插件 🧩
         * @param {AddPluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginAddPost(body?: AddPluginInput, options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
            return SysPluginApiFp(configuration).apiSysPluginAddPost(body, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 添加动态程序集/接口 🧩
         * @param {string} [body] 
         * @param {string} [assemblyName] 程序集名称
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginCompileAssemblyPost(body?: string, assemblyName?: string, options?: AxiosRequestConfig): Promise<AxiosResponse<AdminResultString>> {
            return SysPluginApiFp(configuration).apiSysPluginCompileAssemblyPost(body, assemblyName, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 删除动态插件 🧩
         * @param {DeletePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginDeletePost(body?: DeletePluginInput, options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
            return SysPluginApiFp(configuration).apiSysPluginDeletePost(body, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 获取动态插件列表 🧩
         * @param {PagePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginPagePost(body?: PagePluginInput, options?: AxiosRequestConfig): Promise<AxiosResponse<AdminResultSqlSugarPagedListSysPlugin>> {
            return SysPluginApiFp(configuration).apiSysPluginPagePost(body, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 移除动态程序集/接口 🧩
         * @param {string} assemblyName 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginRemoveAssemblyAssemblyNamePost(assemblyName: string, options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
            return SysPluginApiFp(configuration).apiSysPluginRemoveAssemblyAssemblyNamePost(assemblyName, options).then((request) => request(axios, basePath));
        },
        /**
         * 
         * @summary 更新动态插件 🧩
         * @param {UpdatePluginInput} [body] 
         * @param {*} [options] Override http request option.
         * @throws {RequiredError}
         */
        async apiSysPluginUpdatePost(body?: UpdatePluginInput, options?: AxiosRequestConfig): Promise<AxiosResponse<void>> {
            return SysPluginApiFp(configuration).apiSysPluginUpdatePost(body, options).then((request) => request(axios, basePath));
        },
    };
};

/**
 * SysPluginApi - object-oriented interface
 * @export
 * @class SysPluginApi
 * @extends {BaseAPI}
 */
export class SysPluginApi extends BaseAPI {
    /**
     * 
     * @summary 增加动态插件 🧩
     * @param {AddPluginInput} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysPluginApi
     */
    public async apiSysPluginAddPost(body?: AddPluginInput, options?: AxiosRequestConfig) : Promise<AxiosResponse<void>> {
        return SysPluginApiFp(this.configuration).apiSysPluginAddPost(body, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 添加动态程序集/接口 🧩
     * @param {string} [body] 
     * @param {string} [assemblyName] 程序集名称
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysPluginApi
     */
    public async apiSysPluginCompileAssemblyPost(body?: string, assemblyName?: string, options?: AxiosRequestConfig) : Promise<AxiosResponse<AdminResultString>> {
        return SysPluginApiFp(this.configuration).apiSysPluginCompileAssemblyPost(body, assemblyName, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 删除动态插件 🧩
     * @param {DeletePluginInput} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysPluginApi
     */
    public async apiSysPluginDeletePost(body?: DeletePluginInput, options?: AxiosRequestConfig) : Promise<AxiosResponse<void>> {
        return SysPluginApiFp(this.configuration).apiSysPluginDeletePost(body, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 获取动态插件列表 🧩
     * @param {PagePluginInput} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysPluginApi
     */
    public async apiSysPluginPagePost(body?: PagePluginInput, options?: AxiosRequestConfig) : Promise<AxiosResponse<AdminResultSqlSugarPagedListSysPlugin>> {
        return SysPluginApiFp(this.configuration).apiSysPluginPagePost(body, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 移除动态程序集/接口 🧩
     * @param {string} assemblyName 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysPluginApi
     */
    public async apiSysPluginRemoveAssemblyAssemblyNamePost(assemblyName: string, options?: AxiosRequestConfig) : Promise<AxiosResponse<void>> {
        return SysPluginApiFp(this.configuration).apiSysPluginRemoveAssemblyAssemblyNamePost(assemblyName, options).then((request) => request(this.axios, this.basePath));
    }
    /**
     * 
     * @summary 更新动态插件 🧩
     * @param {UpdatePluginInput} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof SysPluginApi
     */
    public async apiSysPluginUpdatePost(body?: UpdatePluginInput, options?: AxiosRequestConfig) : Promise<AxiosResponse<void>> {
        return SysPluginApiFp(this.configuration).apiSysPluginUpdatePost(body, options).then((request) => request(this.axios, this.basePath));
    }
}
