using NPOI.SS.UserModel;

namespace Pms.Payrolls.ServiceLayer.Files.Exports.Governments
{
    public interface ISheetWriter
    {
        void Write(ISheet sheet, int startIndex = 1);
    }
}
