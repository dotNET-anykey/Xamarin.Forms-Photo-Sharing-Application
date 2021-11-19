using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Renci.SshNet;
using System.Threading.Tasks;
using Renci.SshNet.Async;

namespace PhotoApp
{
    public static class Server
    {
        public static readonly SftpClient Client = new SftpClient("178.219.175.106", "user1", "resu11");
        public const string GalleriesPath = "/web/photoApp/galleries/";
        public const string UsersPath = "/web/photoApp/users/";
        public static string ActiveUserId { get; set; }
        public static string ActiveGalleryId { get; set; }
        public static string ActiveGalleryName { get; set; }


        public static string CreateNewUser()
        {
            var filesInDirectory = Client.ListDirectory(UsersPath).Select(s => s.Name).ToList();
            filesInDirectory.RemoveRange(0, 2);
            int newUserId;

            if (!filesInDirectory.Any())
                newUserId = 1;
            else
            {
                var removedExtensions = new List<string>();
                foreach (var file in filesInDirectory)
                {
                    removedExtensions.Add(file.Remove(file.Length - 4, 4));
                }
                var intList = removedExtensions.Select(int.Parse).ToList();
                newUserId = intList.Max() + 1;
            }
            Client.Create(UsersPath + newUserId + ".csv");

            return "User " + newUserId;
        }


        public static Gallery CreateNewGallery(string galleryName)
        {
            var filesInDirectory = Client.ListDirectory(GalleriesPath).Select(s => s.Name).ToList();
            filesInDirectory.RemoveRange(0, 2);
            int newGalleryId;

            if (!filesInDirectory.Any())
                newGalleryId = 1001;
            else
            {
                var intList = filesInDirectory.Select(int.Parse).ToList();
                newGalleryId = intList.Max() + 1;
            }

            var galleryConfigPath = GalleriesPath + newGalleryId + "/galleryConfig.csv";

            Client.CreateDirectory(GalleriesPath + newGalleryId);
            Client.CreateDirectory(GalleriesPath + newGalleryId + "/Images");
            Client.CreateDirectory(GalleriesPath + newGalleryId + "/Thumbnails");
            Client.Create(galleryConfigPath);

            if (galleryName == "")
            {
                galleryName = "New Gallery";
            }

            Client.WriteAllLines(galleryConfigPath, new[] { galleryName, ActiveUserId + ", " });
            AddGalleryIdToUserFileAsync(ActiveUserId, newGalleryId.ToString());

            return new Gallery(newGalleryId.ToString(), galleryName);
        }


        public static async void DeleteGallery()
        {
            var assignedUsers = await GetAssignedUsersAsync(ActiveGalleryId);

            foreach (var user in assignedUsers)
            {
                RemoveGalleryIdFromUserFileAsync(user, ActiveGalleryId);
            }
            await Task.Run(() => DeleteDirectory(GalleriesPath+ActiveGalleryId));
        }


        public static void RenameGallery(string galleryName)
        {
            var galleryConfigPath = GalleriesPath + ActiveGalleryId + "/galleryConfig.csv";
            var configData = Client.ReadAllLines(galleryConfigPath);
            configData[0] = galleryName;
            Client.Open(galleryConfigPath, FileMode.Truncate);
            Client.WriteAllLines(galleryConfigPath,configData);
            ActiveGalleryName = galleryName;
        }


        public static void AssignUserToGallery(string userId)
        {
            //Adding galleryID to a user CSV file
            AddGalleryIdToUserFileAsync(userId, ActiveGalleryId);
            //Adding userID to a gallery CSV file
            AddUserIdToGalleryFileAsync(userId, ActiveGalleryId);
        }


        public static void RemoveUserFromGallery(string userId)
        {
            //Removing galleryID from a user CSV file
            RemoveGalleryIdFromUserFileAsync(userId, ActiveGalleryId);
            //Removing userID from a gallery CSV file
            RemoveUserIdFromGalleryFileAsync(userId, ActiveGalleryId);
        }


        private static void DeleteDirectory(string path)
        {
            foreach (var file in Client.ListDirectory(path))
            {
                if ((file.Name != ".") && (file.Name != ".."))
                {
                    if (file.IsDirectory)
                    {
                        DeleteDirectory(file.FullName);
                    }
                    else
                    {
                        Client.DeleteFile(file.FullName);
                    }
                }
            }
            Client.DeleteDirectory(path);
        }


        public static async Task<List<string>> GetActiveUserAssignedGalleriesAsync()
        {
            var assignedGalleries = await GetAssignedGalleriesAsync(ActiveUserId);
            return assignedGalleries;
        }


        public static async Task<List<string>> GetActiveGalleryAssignedUsersAsync()
        {
            var assignedUsers = await GetAssignedUsersAsync(ActiveGalleryId);
            return assignedUsers;
        }


        public static async Task<List<string>> GetNotAssignedUsers()
        {
            var allUsers = Client.ListDirectory(UsersPath).Select(s => s.Name).ToList();
            allUsers.RemoveRange(0, 2);
            var assignedUsers = await GetActiveGalleryAssignedUsersAsync();
            var notAssignedUsers = new List<string>();

            foreach (var user in allUsers)
            {
                if (assignedUsers.All(x => x != user.Remove(user.Length - 4, 4)))
                {
                    notAssignedUsers.Add("User " + user.Remove(user.Length - 4, 4));
                }
            }
            return notAssignedUsers;
        }


        private static async Task<List<string>> GetAssignedGalleriesAsync(string userId)
        {
            var userCsvPath = UsersPath + userId + ".csv";
            var userCsvLine = await Task.Run(() => Client.ReadAllText(userCsvPath));
            var assignedGalleries = userCsvLine.Split(new[] { ", " }, StringSplitOptions.None).ToList();
            assignedGalleries.Remove(assignedGalleries.Last());

            return assignedGalleries;
        }


        private static async Task<List<string>> GetAssignedUsersAsync(string galleryId)
        {
            var galleryCsvPath = GalleriesPath + galleryId + "/galleryConfig.csv";
            var configData = await Task.Run(() => Client.ReadAllLines(galleryCsvPath));
            var galleryCsvLine = configData[1]; //Line with user IDs
            var assignedUsers = galleryCsvLine.Split(new[] { ", " }, StringSplitOptions.None).ToList();
            assignedUsers.Remove(assignedUsers.Last());

            return assignedUsers;
        }


        public static List<string> GetAllUsers()
        {
            var allUsersFiles = Client.ListDirectory(UsersPath).Select(s => s.Name).ToList();
            allUsersFiles.RemoveRange(0, 2);
            allUsersFiles.Sort();

            var allUsers = new List<string>();

            foreach (var user in allUsersFiles)
            {
                allUsers.Add("User " + user.Remove(user.Length - 4, 4));
            }

            return allUsers;
        }


        private static async void AddGalleryIdToUserFileAsync(string userId, string galleryId)
        {
            var userCsvPath = UsersPath + userId + ".csv";
            var assignedGalleries = await GetAssignedGalleriesAsync(userId);

            var exists = false;
            foreach (var gallery in assignedGalleries)
            {
                if (gallery == galleryId)
                    exists = true;
            }

            if (!exists)
            {
                var outputString = "";
                assignedGalleries.Add(galleryId);
                assignedGalleries.Sort();
                foreach (var data in assignedGalleries)
                {
                    outputString = outputString + data + ", ";
                }

                Client.Open(userCsvPath, FileMode.Truncate);
                Client.WriteAllText(userCsvPath, outputString);
            }
        }


        private static async void RemoveGalleryIdFromUserFileAsync(string userId, string galleryId)
        {
            var userCsvPath = UsersPath + userId + ".csv";
            var assignedGalleries = await GetAssignedGalleriesAsync(userId);

            foreach (var gallery in assignedGalleries.ToList())
            {
                if (gallery == galleryId)
                {
                    var outputString = "";
                    assignedGalleries.Remove(gallery);

                    foreach (var gallery2 in assignedGalleries)
                    {
                        outputString = outputString + gallery2 + ", ";
                    }

                    Client.Open(userCsvPath, FileMode.Truncate);
                    Client.WriteAllText(userCsvPath, outputString);
                }
            }
        }


        private static async void AddUserIdToGalleryFileAsync(string userId, string galleryId)
        {
            var galleryCsvPath = GalleriesPath + galleryId + "/galleryConfig.csv";
            var assignedUsers = await GetAssignedUsersAsync(galleryId);

            var exists = false;

            foreach (var user in assignedUsers)
            {
                if (user == userId)
                    exists = true;
            }
            if (!exists)
            {
                var outputString = "";
                assignedUsers.Add(userId);
                assignedUsers.Sort();

                foreach (var data in assignedUsers)
                {
                    outputString = outputString + data + ", ";
                }

                Client.Open(galleryCsvPath, FileMode.Truncate);
                Client.WriteAllLines(galleryCsvPath, new[] { ActiveGalleryName, outputString});
            }
        }


        private static async void RemoveUserIdFromGalleryFileAsync(string userId, string galleryId)
        {
            var galleryCsvPath = GalleriesPath + galleryId + "/galleryConfig.csv";
            var assignedUsers = await GetAssignedUsersAsync(galleryId);

            foreach (var user in assignedUsers.ToList())
            {
                if (user == userId)
                {
                    var outputString = "";
                    assignedUsers.Remove(user);

                    foreach (var user2 in assignedUsers)
                    {
                        outputString = outputString + user2 + ", ";
                    }

                    Client.Open(galleryCsvPath, FileMode.Truncate);
                    Client.WriteAllLines(galleryCsvPath, new[] { ActiveGalleryName, outputString });
                }
            }
        }


        public static async Task<Gallery> GetGalleryDataAsync(string galleryId)
        {
            return new Gallery(galleryId, await Task.Run(() => Client.ReadAllLines(GalleriesPath + galleryId + "/galleryConfig.csv")[0]));
        }


        public static List<string> GetImagesNames()
        {
            var imagesPathsList = Client.ListDirectory(GalleriesPath + ActiveGalleryId + "/Images").Select(s => s.Name).ToList();
            imagesPathsList.RemoveRange(0, 2);

            return imagesPathsList;
        }


        public static async Task UploadImage(Stream image, string fileName)
        {
            await Client.UploadAsync(image, GalleriesPath + ActiveGalleryId + "/Images/" + fileName);
            //Client.UploadFile(image, GalleriesPath + ActiveGalleryId + "/Images/" + fileName);
        }


        public static async Task UploadThumbnail(Stream image, string fileName)
        {
            await Client.UploadAsync(image, GalleriesPath + ActiveGalleryId + "/Thumbnails/" + fileName);
            //Client.UploadFile(image, GalleriesPath + ActiveGalleryId + "/Thumbnails/" + fileName);
        }


        public static Stream GetImageStream(string path)
        {
            
            return Client.OpenRead(path);
        }


        public static byte[] GetImageInBytes(string path)
        {
            return Client.ReadAllBytes(path);
        }
    }
}
