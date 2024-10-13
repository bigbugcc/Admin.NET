// Admin.NET 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 和 LICENSE-APACHE 文件。
//
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Renci.SshNet;

namespace Admin.NET.Core
{
    /// <summary>
    /// SSH/Sftp 工具类
    /// </summary>
    public class SSHHelper : IDisposable
    {
        private readonly SftpClient _sftp;

        public SSHHelper(string host, int port, string user, string password)
        {
            _sftp = new SftpClient(host, port, user, password);
        }

        /// <summary>
        /// 连接
        /// </summary>
        private void Connect()
        {
            if (!_sftp.IsConnected)
                _sftp.Connect();
        }

        /// <summary>
        /// 是否存在同名文件
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <returns></returns>
        public bool Exists(string ftpFileName)
        {
            Connect();

            return _sftp.Exists(ftpFileName);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="ftpFileName"></param>
        public void DeleteFile(string ftpFileName)
        {
            Connect();

            _sftp.DeleteFile(ftpFileName);
        }

        /// <summary>
        /// 下载到指定目录
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <param name="localFileName"></param>
        public void DownloadFile(string ftpFileName, string localFileName)
        {
            Connect();

            using (Stream fileStream = File.OpenWrite(localFileName))
            {
                _sftp.DownloadFile(ftpFileName, fileStream);
            }
        }

        /// <summary>
        /// 读取字节
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <returns></returns>
        public byte[] ReadAllBytes(string ftpFileName)
        {
            Connect();

            return _sftp.ReadAllBytes(ftpFileName);
        }

        /// <summary>
        /// 读取流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Stream OpenRead(string path)
        {
            return _sftp.Open(path, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        /// <param name="ftpFileName"></param>
        /// <param name="localFileName"></param>
        public void DownloadFileWithResume(string ftpFileName, string localFileName)
        {
            DownloadFile(ftpFileName, localFileName);
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public void RenameFile(string oldPath, string newPath)
        {
            _sftp.RenameFile(oldPath, newPath);
        }

        /// <summary>
        /// 指定目录下文件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<string> GetFileList(string folder, IEnumerable<string> filters)
        {
            Connect();

            var files = new List<string>();
            var sftpFiles = _sftp.ListDirectory(folder);
            foreach (var file in sftpFiles)
            {
                if (file.IsRegularFile && filters.Any(f => file.Name.EndsWith(f)))
                    files.Add(file.Name);
            }
            return files;
        }

        /// <summary>
        /// 上传指定目录文件
        /// </summary>
        /// <param name="localFileName"></param>
        /// <param name="ftpFileName"></param>
        public void UploadFile(string localFileName, string ftpFileName)
        {
            Connect();

            var dir = Path.GetDirectoryName(ftpFileName);
            CreateDir(_sftp, dir);
            using (var fileStream = new FileStream(localFileName, FileMode.Open))
            {
                _sftp.UploadFile(fileStream, ftpFileName);
            }
        }

        /// <summary>
        /// 上传字节
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="ftpFileName"></param>
        public void UploadFile(byte[] bs, string ftpFileName)
        {
            Connect();

            var dir = Path.GetDirectoryName(ftpFileName);
            CreateDir(_sftp, dir);
            _sftp.WriteAllBytes(ftpFileName, bs);
        }

        /// <summary>
        /// 上传流
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="ftpFileName"></param>
        public void UploadFile(Stream fileStream, string ftpFileName)
        {
            Connect();

            var dir = Path.GetDirectoryName(ftpFileName);
            CreateDir(_sftp, dir);
            _sftp.UploadFile(fileStream, ftpFileName);
            fileStream.Dispose();
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="sftp"></param>
        /// <param name="dir"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private void CreateDir(SftpClient sftp, string dir)
        {
            ArgumentNullException.ThrowIfNull(dir);

            if (sftp.Exists(dir)) return;

            var index = dir.LastIndexOfAny(new char[] { '/', '\\' });
            if (index > 0)
            {
                var p = dir[..index];
                if (!sftp.Exists(p))
                    CreateDir(sftp, p);
                sftp.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            if (_sftp == null) return;

            if (_sftp.IsConnected)
                _sftp.Disconnect();
            _sftp.Dispose();
        }
    }
}