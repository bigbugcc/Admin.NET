// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Aliyun.OSS.Util;
using Furion.VirtualFileServer;
using OnceMi.AspNetCore.OSS;

namespace Admin.NET.Core.Service;

/// <summary>
/// 系统文件服务 🧩
/// </summary>
[ApiDescriptionSettings(Order = 410)]
public class SysFileService : IDynamicApiController, ITransient
{
    private readonly UserManager _userManager;
    private readonly SqlSugarRepository<SysFile> _sysFileRep;
    private readonly OSSProviderOptions _OSSProviderOptions;
    private readonly UploadOptions _uploadOptions;
    private readonly IOSSService _OSSService;
    private readonly string _imageType = ".jpeg.jpg.png.bmp.gif.tif";

    public SysFileService(UserManager userManager,
        SqlSugarRepository<SysFile> sysFileRep,
        IOptions<OSSProviderOptions> oSSProviderOptions,
        IOptions<UploadOptions> uploadOptions,
        IOSSServiceFactory ossServiceFactory)
    {
        _userManager = userManager;
        _sysFileRep = sysFileRep;
        _OSSProviderOptions = oSSProviderOptions.Value;
        _uploadOptions = uploadOptions.Value;
        if (_OSSProviderOptions.IsEnable)
            _OSSService = ossServiceFactory.Create(Enum.GetName(_OSSProviderOptions.Provider));
    }

    /// <summary>
    /// 获取文件分页列表 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("获取文件分页列表")]
    public async Task<SqlSugarPagedList<SysFile>> Page(PageFileInput input)
    {
        return await _sysFileRep.AsQueryable()
            .WhereIF(!string.IsNullOrWhiteSpace(input.FileName), u => u.FileName.Contains(input.FileName.Trim()))
            .WhereIF(!string.IsNullOrWhiteSpace(input.StartTime.ToString()) && !string.IsNullOrWhiteSpace(input.EndTime.ToString()),
                        u => u.CreateTime >= input.StartTime && u.CreateTime <= input.EndTime)
            .OrderBy(u => u.CreateTime, OrderByType.Desc)
            .ToPagedListAsync(input.Page, input.PageSize);
    }

    /// <summary>
    /// 上传文件 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("上传文件")]
    public async Task<SysFile> UploadFile([FromForm] FileUploadInput input)
    {
        return await HandleUploadFile(input.File, input.Path, fileType: input.FileType);
    }

    /// <summary>
    /// 上传文件Base64 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("上传文件Base64")]
    public async Task<SysFile> UploadFileFromBase64(UploadFileFromBase64Input input)
    {
        byte[] fileData = Convert.FromBase64String(input.FileDataBase64);
        var ms = new MemoryStream();
        ms.Write(fileData);
        ms.Seek(0, SeekOrigin.Begin);
        if (string.IsNullOrEmpty(input.FileName))
            input.FileName = $"{YitIdHelper.NextId()}.jpg";
        if (string.IsNullOrEmpty(input.ContentType))
            input.ContentType = "image/jpg";
        IFormFile formFile = new FormFile(ms, 0, fileData.Length, "file", input.FileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = input.ContentType
        };
        return await UploadFile(new FileUploadInput { File = formFile, Path = input.Path, FileType = input.FileType });
    }

    /// <summary>
    /// 上传多文件 🔖
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [DisplayName("上传多文件")]
    public async Task<List<SysFile>> UploadFiles([Required] List<IFormFile> files)
    {
        var filelist = new List<SysFile>();
        foreach (var file in files)
        {
            filelist.Add(await UploadFile(new FileUploadInput { File = file }));
        }
        return filelist;
    }

    /// <summary>
    /// 根据文件Id或Url下载 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [DisplayName("根据文件Id或Url下载")]
    public async Task<IActionResult> DownloadFile(FileInput input)
    {
        var _sysFileRepNew = _sysFileRep.CopyNew(); // DownloadFile函数有可能在多线程下使用，所以这里要创建一个新连接
        var file = input.Id > 0 ? await GetFile(input) : await _sysFileRepNew.GetFirstAsync(u => u.Url == input.Url);
        var fileName = HttpUtility.UrlEncode(file.FileName, Encoding.GetEncoding("UTF-8"));

        if (_OSSProviderOptions.IsEnable)
        {
            var filePath = string.Concat(file.FilePath, "/", file.Id.ToString() + file.Suffix);
            var stream = await (await _OSSService.PresignedGetObjectAsync(file.BucketName.ToString(), filePath, 5)).GetAsStreamAsync();
            return new FileStreamResult(stream.Stream, "application/octet-stream") { FileDownloadName = fileName + file.Suffix };
        }
        else if (App.Configuration["SSHProvider:IsEnable"].ToBoolean())
        {
            var fullPath = string.Concat(file.FilePath, "/", file.Id + file.Suffix);
            using (SSHHelper helper = new SSHHelper(App.Configuration["SSHProvider:Host"],
               App.Configuration["SSHProvider:Port"].ToInt(), App.Configuration["SSHProvider:Username"], App.Configuration["SSHProvider:Password"]))
            {
                return new FileStreamResult(helper.OpenRead(fullPath), "application/octet-stream") { FileDownloadName = fileName + file.Suffix };
            }
        }
        else
        {
            var filePath = Path.Combine(file.FilePath, file.Id.ToString() + file.Suffix);
            var path = Path.Combine(App.WebHostEnvironment.WebRootPath, filePath);
            return new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream") { FileDownloadName = fileName + file.Suffix };
        }
    }

    /// <summary>
    /// 文件预览
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [AllowAnonymous]
    public async Task<IActionResult> GetPreview([FromRoute] long Id)
    {
        var file = await GetFile(new FileInput { Id = Id });
        //var fileName = HttpUtility.UrlEncode(file.FileName, Encoding.GetEncoding("UTF-8"));

        if (_OSSProviderOptions.IsEnable)
        {
            var filePath = string.Concat(file.FilePath, "/", file.Id.ToString() + file.Suffix);
            var stream = await (await _OSSService.PresignedGetObjectAsync(file.BucketName.ToString(), filePath, 5)).GetAsStreamAsync();
            return new FileStreamResult(stream.Stream, "application/octet-stream");
        }
        else if (App.Configuration["SSHProvider:IsEnable"].ToBoolean())
        {
            var fullPath = string.Concat(file.FilePath, "/", file.Id + file.Suffix);
            using (SSHHelper helper = new SSHHelper(App.Configuration["SSHProvider:Host"],
               App.Configuration["SSHProvider:Port"].ToInt(), App.Configuration["SSHProvider:Username"], App.Configuration["SSHProvider:Password"]))
            {
                return new FileStreamResult(helper.OpenRead(fullPath), "application/octet-stream");
            }
        }
        else
        {
            var filePath = Path.Combine(file.FilePath, file.Id.ToString() + file.Suffix);
            var path = Path.Combine(App.WebHostEnvironment.WebRootPath, filePath);
            return new FileStreamResult(new FileStream(path, FileMode.Open), "application/octet-stream");
        }
    }

    /// <summary>
    /// 下载指定文件Base64格式 🔖
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [DisplayName("下载指定文件Base64格式")]
    public async Task<string> DownloadFileBase64([FromBody] string url)
    {
        var _sysFileRepNew = _sysFileRep.CopyNew();// DownloadFileBase64函数有可能在多线程下使用，所以这里要创建一个新连接
        if (_OSSProviderOptions.IsEnable)
        {
            using var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                // 读取文件内容并将其转换为 Base64 字符串
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                return Convert.ToBase64String(fileBytes);
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
            }
        }
        else if (App.Configuration["SSHProvider:IsEnable"].ToBoolean())
        {
            var sysFile = await _sysFileRepNew.GetFirstAsync(u => u.Url == url) ?? throw Oops.Oh($"文件不存在");
            using (SSHHelper helper = new SSHHelper(App.Configuration["SSHProvider:Host"],
               App.Configuration["SSHProvider:Port"].ToInt(), App.Configuration["SSHProvider:Username"], App.Configuration["SSHProvider:Password"]))
            {
                return Convert.ToBase64String(helper.ReadAllBytes(sysFile.FilePath));
            }
        }
        else
        {
            var sysFile = await _sysFileRepNew.GetFirstAsync(u => u.Url == url) ?? throw Oops.Oh($"文件不存在");
            var filePath = Path.Combine(App.WebHostEnvironment.WebRootPath, sysFile.FilePath);
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            var realFile = Path.Combine(filePath, $"{sysFile.Id}{sysFile.Suffix}");
            if (!File.Exists(realFile))
            {
                Log.Error($"DownloadFileBase64:文件[{realFile}]不存在");
                throw Oops.Oh($"文件[{sysFile.FilePath}]不存在");
            }
            byte[] fileBytes = File.ReadAllBytes(realFile);
            return Convert.ToBase64String(fileBytes);
        }
    }

    /// <summary>
    /// 删除文件 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Delete"), HttpPost]
    [DisplayName("删除文件")]
    public async Task DeleteFile(DeleteFileInput input)
    {
        var file = await _sysFileRep.GetFirstAsync(u => u.Id == input.Id);
        if (file != null)
        {
            await _sysFileRep.DeleteAsync(file);

            if (_OSSProviderOptions.IsEnable)
            {
                await _OSSService.RemoveObjectAsync(file.BucketName.ToString(), string.Concat(file.FilePath, "/", $"{input.Id}{file.Suffix}"));
            }
            else if (App.Configuration["SSHProvider:IsEnable"].ToBoolean())
            {
                var fullPath = string.Concat(file.FilePath, "/", file.Id + file.Suffix);
                using (SSHHelper helper = new SSHHelper(App.Configuration["SSHProvider:Host"],
                   App.Configuration["SSHProvider:Port"].ToInt(), App.Configuration["SSHProvider:Username"], App.Configuration["SSHProvider:Password"]))
                {
                    helper.DeleteFile(fullPath);
                }
            }
            else
            {
                var filePath = Path.Combine(App.WebHostEnvironment.WebRootPath, file.FilePath, input.Id.ToString() + file.Suffix);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }
    }

    /// <summary>
    /// 更新文件 🔖
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [ApiDescriptionSettings(Name = "Update"), HttpPost]
    [DisplayName("更新文件")]
    public async Task UpdateFile(FileInput input)
    {
        var isExist = await _sysFileRep.IsAnyAsync(u => u.Id == input.Id);
        if (!isExist) throw Oops.Oh(ErrorCodeEnum.D8000);

        await _sysFileRep.UpdateAsync(u => new SysFile() { FileName = input.FileName, FileType = input.FileType }, u => u.Id == input.Id);
    }

    /// <summary>
    /// 获取文件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private async Task<SysFile> GetFile([FromQuery] FileInput input)
    {
        var file = await _sysFileRep.GetFirstAsync(u => u.Id == input.Id);
        return file ?? throw Oops.Oh(ErrorCodeEnum.D8000);
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <param name="savePath">路径</param>
    /// <param name="allowSuffix">允许格式：.jpg.png.gif.tif.bmp</param>
    /// <param name="fileType">类型</param>
    /// <returns></returns>
    private async Task<SysFile> HandleUploadFile(IFormFile file, string savePath, string allowSuffix = "", string fileType = "")
    {
        if (file == null) throw Oops.Oh(ErrorCodeEnum.D8000);

        // 判断是否重复上传的文件
        var sizeKb = (long)(file.Length / 1024.0); // 大小KB
        var fileMd5 = string.Empty;
        if (_uploadOptions.EnableMd5)
        {
            using (var fileStream = file.OpenReadStream())
            {
                fileMd5 = OssUtils.ComputeContentMd5(fileStream, fileStream.Length);
            }
            /*
             * Mysql8 中如果使用了 utf8mb4_general_ci 之外的编码会出错，尽量避免在条件里使用.ToString()
             * 因为 Squsugar 并不是把变量转换为字符串来构造SQL语句，而是构造了CAST(123 AS CHAR)这样的语句，这样这个返回值是utf8mb4_general_ci，所以容易出错。
             */
            var sysFile = await _sysFileRep.GetFirstAsync(u => u.FileMd5 == fileMd5 && u.SizeKb == sizeKb);
            if (sysFile != null) return sysFile;
        }

        // 验证文件类型
        if (!_uploadOptions.ContentType.Contains(file.ContentType))
            throw Oops.Oh(ErrorCodeEnum.D8001);

        // 验证文件大小
        if (sizeKb > _uploadOptions.MaxSize)
            throw Oops.Oh(ErrorCodeEnum.D8002);

        // 获取文件后缀
        var suffix = Path.GetExtension(file.FileName).ToLower(); // 后缀
        if (!string.IsNullOrWhiteSpace(suffix))
        {
            //var contentTypeProvider = FS.GetFileExtensionContentTypeProvider();
            //suffix = contentTypeProvider.Mappings.FirstOrDefault(u => u.Value == file.ContentType).Key;
            // 修改 image/jpeg 类型返回的 .jpeg、jpe 后缀
            if (suffix == ".jpeg" || suffix == ".jpe")
                suffix = ".jpg";
        }
        
        // 获取后缀名失败 并且 上传文件为 blob
        if (suffix == "" && file.FileName == "blob")
            suffix = "." + file.ContentType.Substring(file.ContentType.LastIndexOf('/') + 1); // file.ContentType.Split('/')[1]

        if (string.IsNullOrWhiteSpace(suffix))
            throw Oops.Oh(ErrorCodeEnum.D8003);

        // 防止客户端伪造文件类型
        if (!string.IsNullOrWhiteSpace(allowSuffix) && !allowSuffix.Contains(suffix))
            throw Oops.Oh(ErrorCodeEnum.D8003);
        if (!VerifyFileExtensionName.IsSameType(file.OpenReadStream(), suffix))
            throw Oops.Oh(ErrorCodeEnum.D8001);

        var path = string.IsNullOrWhiteSpace(savePath) ? _uploadOptions.Path : savePath;
        path = path.ParseToDateTimeForRep();
        var newFile = new SysFile
        {
            Id = YitIdHelper.NextId(),
            // BucketName = _OSSProviderOptions.IsEnable ? _OSSProviderOptions.Provider.ToString() : "Local",
            // 阿里云对bucket名称有要求，1.只能包括小写字母，数字，短横线（-）2.必须以小写字母或者数字开头  3.长度必须在3-63字节之间
            // 无法使用Provider
            BucketName = _OSSProviderOptions.IsEnable ? _OSSProviderOptions.Bucket : "Local",
            FileName = Path.GetFileNameWithoutExtension(file.FileName),
            Suffix = suffix,
            SizeKb = sizeKb,
            FilePath = path,
            FileMd5 = fileMd5,
            FileType = fileType
        };

        var finalName = newFile.Id + suffix; // 文件最终名称
        if (_OSSProviderOptions.IsEnable)
        {
            newFile.Provider = Enum.GetName(_OSSProviderOptions.Provider);
            var filePath = string.Concat(path, "/", finalName);
            await _OSSService.PutObjectAsync(newFile.BucketName, filePath, file.OpenReadStream());
            //  http://<你的bucket名字>.oss.aliyuncs.com/<你的object名字>
            //  生成外链地址 方便前端预览
            switch (_OSSProviderOptions.Provider)
            {
                case OSSProvider.Aliyun:
                    newFile.Url = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{newFile.BucketName}.{_OSSProviderOptions.Endpoint}/{filePath}";
                    break;

                case OSSProvider.Minio:
                    // 获取Minio文件的下载或者预览地址
                    // newFile.Url = await GetMinioPreviewFileUrl(newFile.BucketName, filePath);// 这种方法生成的Url是有7天有效期的，不能这样使用
                    // 需要在MinIO中的Buckets开通对 Anonymous 的readonly权限
                    newFile.Url = $"{(_OSSProviderOptions.IsEnableHttps ? "https" : "http")}://{_OSSProviderOptions.Endpoint}/{newFile.BucketName}/{filePath}";
                    break;
            }
        }
        else if (App.Configuration["SSHProvider:IsEnable"].ToBoolean())
        {
            var fullPath = string.Concat(path.StartsWith("/") ? path : "/" + path, "/", finalName);
            using (SSHHelper helper = new SSHHelper(App.Configuration["SSHProvider:Host"],
               App.Configuration["SSHProvider:Port"].ToInt(), App.Configuration["SSHProvider:Username"], App.Configuration["SSHProvider:Password"]))
            {
                helper.UploadFile(file.OpenReadStream(), fullPath);
            }
        }
        else
        {
            newFile.Provider = ""; // 本地存储 Provider 显示为空
            var filePath = Path.Combine(App.WebHostEnvironment.WebRootPath, path);
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            var realFile = Path.Combine(filePath, finalName);
            using (var stream = File.Create(realFile))
            {
                await file.CopyToAsync(stream);
            }

            // 生成外链
            var host = CommonUtil.GetLocalhost();
            if (!host.EndsWith('/'))
                host += "/";
            newFile.Url = $"{host}{newFile.FilePath}/{newFile.Id + newFile.Suffix}";
        }
        await _sysFileRep.AsInsertable(newFile).ExecuteCommandAsync();
        return newFile;
    }

    ///// <summary>
    ///// 获取Minio文件的下载或者预览地址
    ///// </summary>
    ///// <param name="bucketName">桶名</param>
    ///// <param name="fileName">文件名</param>
    ///// <returns></returns>
    //private async Task<string> GetMinioPreviewFileUrl(string bucketName, string fileName)
    //{
    //    return await _OSSService.PresignedGetObjectAsync(bucketName, fileName, 7);
    //}

    /// <summary>
    /// 上传头像 🔖
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [DisplayName("上传头像")]
    public async Task<SysFile> UploadAvatar([Required] IFormFile file)
    {
        var sysFile = await HandleUploadFile(file, "Upload/Avatar", _imageType);

        var sysUserRep = _sysFileRep.ChangeRepository<SqlSugarRepository<SysUser>>();
        var user = sysUserRep.GetFirst(u => u.Id == _userManager.UserId);
        // 删除已有头像文件
        if (!string.IsNullOrWhiteSpace(user.Avatar))
        {
            var fileId = Path.GetFileNameWithoutExtension(user.Avatar);
            await DeleteFile(new DeleteFileInput { Id = long.Parse(fileId) });
        }
        await sysUserRep.UpdateAsync(u => new SysUser() { Avatar = sysFile.Url }, u => u.Id == user.Id);
        return sysFile;
    }

    /// <summary>
    /// 上传电子签名 🔖
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [DisplayName("上传电子签名")]
    public async Task<SysFile> UploadSignature([Required] IFormFile file)
    {
        var sysFile = await HandleUploadFile(file, "Upload/Signature", _imageType);

        var sysUserRep = _sysFileRep.ChangeRepository<SqlSugarRepository<SysUser>>();
        var user = sysUserRep.GetFirst(u => u.Id == _userManager.UserId);
        // 删除已有电子签名文件
        if (!string.IsNullOrWhiteSpace(user.Signature) && user.Signature.EndsWith(".png"))
        {
            var fileId = Path.GetFileNameWithoutExtension(user.Signature);
            await DeleteFile(new DeleteFileInput { Id = long.Parse(fileId) });
        }
        await sysUserRep.UpdateAsync(u => new SysUser() { Signature = sysFile.Url }, u => u.Id == user.Id);
        return sysFile;
    }

    /// <summary>
    /// 修改附件关联对象
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="relationName"></param>
    /// <param name="relationId"></param>
    /// <param name="belongId"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<int> UpdateRelation(List<long> ids, string relationName, long relationId, long belongId = 0)
    {
        if (ids == null || ids.Count == 0)
            return 0;
        return await _sysFileRep.AsUpdateable()
              .SetColumns(m => m.RelationName == relationName)
              .SetColumns(m => m.RelationId == relationId)
              .SetColumns(m => m.BelongId == belongId)
             .Where(m => ids.Contains(m.Id))
             .ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据关联查询附件
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<List<FileOutput>> GetRelationFiles([FromQuery] RelationQueryInput input)
    {
        return await _sysFileRep.AsQueryable()
           .Where(m => !m.IsDelete)
           .WhereIF(input.RelationId.HasValue && input.RelationId > 0, m => m.RelationId == input.RelationId)
           .WhereIF(input.BelongId.HasValue && input.BelongId > 0, m => m.BelongId == input.BelongId.Value)
           .WhereIF(!string.IsNullOrWhiteSpace(input.RelationName), m => m.RelationName == input.RelationName)
           .WhereIF(!string.IsNullOrWhiteSpace(input.FileTypes), m => input.GetFileTypeBS().Contains(m.FileType))
            .Select(m => new FileOutput
            {
                Id = m.Id,
                FileType = m.FileType,
                Name = m.FileName,
                RelationId = m.RelationId,
                BelongId = m.BelongId,
                FilePath = m.FilePath,
                SizeKb = m.SizeKb,
                Suffix = m.Suffix,
                RelationName = m.RelationName,
                Url = SqlFunc.MergeString("/api/sysFile/Preview/", m.Id.ToString()),
                CreateUserName = m.CreateUserName,
                CreateTime = m.CreateTime,
            })
           .ToListAsync();
    }
}