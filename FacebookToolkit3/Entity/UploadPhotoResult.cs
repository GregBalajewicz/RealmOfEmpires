using System;
using System.Collections.Generic;
using System.Text;

namespace Facebook.Entity
{
    public class UploadPhotoResult
    {
        private string _photoId;

        private string _albumId;

        public UploadPhotoResult(string photoId, string albumId)
        {
            _photoId = photoId;
            _albumId = albumId;
        }

        /// <summary>
        /// The id of the uploaded photo
        /// </summary>
        public string PhotoId
        {
            get { return _photoId; }
        }

        /// <summary>
        /// The id of the album the photo was uploaded to.
        /// </summary>
        public string AlbumId
        {
            get { return _albumId; }
        }
    }
}
