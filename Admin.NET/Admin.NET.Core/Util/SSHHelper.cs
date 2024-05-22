
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace Admin.NET.Core
{
    /// <summary>
    /// SSH / Sftp Helper
    /// </summary>
    public class SSHHelper : IDisposable
    {
        private SftpClient sftp;
        public SSHHelper(string host, int port, string user, string password)
        {
            sftp = new SftpClient(host, port, user, password);
        }
        public bool Exists(string ftpFileName)
        {
            Connect();
            return sftp.Exists(ftpFileName);

        }

        private void Connect()
        {
            if (!sftp.IsConnected)
            {
                sftp.Connect();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ftpFileName"></param>
        public void DeleteFile(string ftpFileName)
        { 
            Connect();
            sftp.DeleteFile(ftpFileName);

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
                sftp.DownloadFile(ftpFileName, fileStream);
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
            return sftp.ReadAllBytes(ftpFileName);
        }
        /// <summary>
        /// 读取流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Stream OpenRead(string path)
        {
            return sftp.Open(path, FileMode.Open, FileAccess.Read);
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
            sftp.RenameFile(oldPath, newPath);
        }

        /// <summary>
        /// 指定目录下文件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public List<string> GetFileList(string folder, IEnumerable<string> filters)
        {
            var files = new List<string>();
            Connect();

            var sftpFiles = sftp.ListDirectory(folder);

            foreach (var file in sftpFiles)
            {
                if (file.IsRegularFile && filters.Any(f => file.Name.EndsWith(f)))
                {
                    files.Add(file.Name);
                }
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
            CreateDir(sftp, dir);
            using (var fileStream = new FileStream(localFileName, FileMode.Open))
            {
                sftp.UploadFile(fileStream, ftpFileName);
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
            CreateDir(sftp, dir);

            sftp.WriteAllBytes(ftpFileName, bs);
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
            CreateDir(sftp, dir);
            sftp.UploadFile(fileStream, ftpFileName);
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
            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }
            if (!sftp.Exists(dir))
            {
                var index = dir.LastIndexOfAny(new char[] { '/', '\\' });
                if (index > 0)
                {
                    var p = dir.Substring(0, index);
                    if (!sftp.Exists(p))
                    {
                        CreateDir(sftp, p);
                    }
                    sftp.CreateDirectory(dir);
                }
            }
        }

        public void Dispose()
        {
            if (sftp != null)
            {
                if (sftp.IsConnected)
                    sftp.Disconnect();
                sftp.Dispose();
            }
        }
    }
}
