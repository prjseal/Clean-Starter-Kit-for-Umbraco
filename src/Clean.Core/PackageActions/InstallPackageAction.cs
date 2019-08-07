using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.PackageActions;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Core.Services;
using Umbraco.Web.Composing;

namespace Clean.Core.PackageActions
{
    public class CleanStarterKitInstallPackageAction : IPackageAction
    {
        public bool Execute(string packageName, XElement xmlData)
        {
            var contentService = Current.Services.ContentService;
            var mediaTypeService = Current.Services.MediaTypeService;
            var mediaService = Current.Services.MediaService;

            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("3c6c415c-35a0-4629-891e-683506250c31"), "1", "/media/ljahypfa/1.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("5598b628-b390-4532-8bb5-dab06089e9d7"), "2", "/media/f01jqvmq/2.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("bbf2800f-1cc5-4ea9-8d2e-b33ff1d5efbe"), "3", "/media/bgapfo4f/3.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("48239f24-1450-49da-9964-934f512dde48"), "4", "/media/ekpdgscq/4.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("167ee715-53ff-4a8b-ab50-7d630c8448fa"), "5", "/media/cpth2fx5/5.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("b5279cc9-c438-4b14-a57a-72f97f5527f7"), "6", "/media/z4ocpnjy/6.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("a61578a3-11bb-4425-9fe1-86a89434e638"), "7", "/media/uv2bljv1/7.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("da20812c-977d-4707-80d9-f2b1cb185ff1"), "8", "/media/fkadbwt3/8.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("0521b485-98b6-409d-ad35-8745f3e4287c"), "9", "/media/qo0ccbnw/9.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("8ac2c7bc-0acb-488e-a4e6-24d9ea5bdff7"), "10", "/media/xldnfcwl/10.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("13e63ebb-2f82-4e65-84f0-e3b79241a971"), "11", "/media/xu5ome2c/11.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("dd3840b3-8621-451b-9ee9-01f6ad175ec7"), "12", "/media/wvadrxjb/12.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("db22bb69-b0ae-44bb-9019-a3e6498d4c25"), "13", "/media/n43dfw3o/13.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("4281aa58-15c6-41a9-b074-33f40fd09ea9"), "14", "/media/l0vicqwg/14.jpg", false);
            CreateMediaItem(mediaService, mediaTypeService, -1, "image", new Guid("861ac5a3-aca5-4db8-85f2-a7ad05e0de30"), "15", "/media/uxcbvbvf/15.jpg", false);

            var contentHome = contentService.GetRootContent().FirstOrDefault(x => x.ContentType.Alias == "home");
            if (contentHome != null)
            {
                // publish everything (moved here due to Deploy dependency checking)
                contentService.SaveAndPublishBranch(contentHome, true);
            }
            else
            {
                Current.Logger.Warn<CleanStarterKitInstallPackageAction>("The installed Home page was not found");
            }

            return true;
        }

        public string Alias()
        {
            return "CleanStarterKitMedia";
        }

        public bool Undo(string packageName, XElement xmlData)
        {
            //no undo path
            return true;
        }

        private int CreateMediaItem(IMediaService service, IMediaTypeService mediaTypeService,
            int parentFolderId, string nodeTypeAlias, Guid key, string nodeName, string mediaPath, bool checkForDuplicateName = false)
        {
            //if the item with the exact id exists we cannot install it (the package was probably already installed)
            if (service.GetById(key) != null)
                return -1;

            //cannot continue if the media type doesn't exist
            var mediaType = mediaTypeService.Get(nodeTypeAlias);
            if (mediaType == null)
            {
                Current.Logger.Warn<CleanStarterKitInstallPackageAction>("Could not create media, the {NodeTypeAlias} media type is missing, the Clean Starter Kit package will not function correctly", nodeTypeAlias);
                return -1;
            }

            var isDuplicate = false;

            if (checkForDuplicateName)
            {
                IEnumerable<IMedia> children;
                if (parentFolderId == -1)
                {
                    children = service.GetRootMedia();
                }
                else
                {
                    var parentFolder = service.GetById(parentFolderId);
                    if (parentFolder == null)
                    {
                        Current.Logger.Warn<CleanStarterKitInstallPackageAction>("No media parent found by Id {ParentFolderId} the media item {NodeName} cannot be installed", parentFolderId, nodeName);
                        return -1;
                    }

                    children = service.GetPagedChildren(parentFolderId, 0, int.MaxValue, out long totalRecords);
                }
                foreach (var m in children)
                {
                    if (m.Name == nodeName)
                    {
                        isDuplicate = true;
                        break;
                    }
                }
            }

            if (isDuplicate) return -1;

            if (parentFolderId != -1)
            {
                var parentFolder = service.GetById(parentFolderId);
                if (parentFolder == null)
                {
                    Current.Logger.Warn<CleanStarterKitInstallPackageAction>("No media parent found by Id {ParentFolderId} the media item {NodeName} cannot be installed", parentFolderId, nodeName);
                    return -1;
                }
            }

            var media = service.CreateMedia(nodeName, parentFolderId, nodeTypeAlias);
            if (nodeTypeAlias != "folder")
                media.SetValue("umbracoFile", JsonConvert.SerializeObject(new ImageCropperValue { Src = mediaPath }));
            if (key != Guid.Empty)
            {
                media.Key = key;
            }
            service.Save(media);
            return media.Id;
        }
    }
}