using System.Collections.Generic;

namespace Warranty.Core.FileManagement
{
    public interface IFileSystemManager<TEntity> 
    {
        List<string> MoveFilesToDirectoryAndRenameToAvoidCollisions(TEntity entity, IEnumerable<string> temporaryFileIds, string newFriendlyName = null);

        List<string> MoveFilesToDirectoryAndRenameToAvoidCollisions(IEnumerable<string> temporaryFileIds, string newFriendlyName = null);

        void DeleteAttachment(TEntity entity, string fileName);
    }
}