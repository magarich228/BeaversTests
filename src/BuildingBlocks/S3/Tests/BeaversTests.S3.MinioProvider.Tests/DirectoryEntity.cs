using System.Text;
using BeaversTests.Common.Binary;
using Newtonsoft.Json;

namespace BeaversTests.S3.MinioProvider.Tests;

public class DirectoryEntity : FileSystemEntity
{
    public static DirectoryEntity Create()
    {
        var testFile1Content = Encoding.UTF8.GetBytes(typeof(DirectoryEntity).ToString());
        var testFile2Content = Encoding.UTF8.GetBytes(new Exception("test").ToString());
        var testFile3Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Global.ServiceProvider));
        
        return new DirectoryEntity
        {
            Files = new[]
            {
                new BeaversTestsFile()
                {
                    Name = "test-file1.txt",
                    Content = testFile1Content,
                    Length = testFile1Content.Length,
                    MediaType = "plain/text"
                },
                new BeaversTestsFile()
                {
                    Name = "test-file2",
                    Content = testFile2Content,
                    Length = testFile2Content.Length,
                    MediaType = "application/octet-stream"
                }
            },
            Directories = new[]
            {
                new BeaversTestsDirectory()
                {
                    Directories = new[]
                    {
                        new BeaversTestsDirectory()
                        {
                            DirectoryName = "sub-dir",
                            Directories = Array.Empty<BeaversTestsDirectory>(),
                            TestFiles = Array.Empty<BeaversTestsFile>()
                        }
                    },
                    DirectoryName = "sub-dir2",
                    TestFiles = new[]
                    {
                        new BeaversTestsFile()
                        {
                            Name = "test-file3",
                            Content = testFile3Content,
                            Length = testFile3Content.Length,
                            MediaType = "application/json"
                        },
                        new BeaversTestsFile()
                        {
                            Name = "test-file3.1",
                            Content = testFile3Content,
                            Length = testFile3Content.Length,
                            MediaType = "application/json"
                        }
                    }
                }
            }
        };
    }
}