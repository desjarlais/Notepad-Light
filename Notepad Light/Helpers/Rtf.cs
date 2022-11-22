using System.Text;

namespace Notepad_Light.Helpers
{
    public class Rtf
    {
        /// <summary>
        /// create an rtf table
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string InsertTable(int rows, int cols, int width)
        {
            StringBuilder sb = new StringBuilder();

            // rtf start
            sb.Append(@"{\rtf1 ");

            int cellWidth;

            // start row
            sb.Append(@"\trowd");

            for (int i = 0; i < rows; i++)
            {
                sb.Append(@"\trowd");
                for (int j = 0; j < cols; j++)
                {
                    cellWidth = (j + 1) * width;
                    sb.Append(@"\cellx" + cellWidth.ToString());
                }

                sb.Append(@"\intbl \cell \row");
            }

            sb.Append(@"\pard");
            sb.Append('}');

            return sb.ToString();
        }
    }
}
