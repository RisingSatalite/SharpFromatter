namespace SharpFromatter;

using System.IO;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        OpenFileDialog openFileDialog = new OpenFileDialog();

        openFileDialog.Title = "Select a file";
        openFileDialog.Filter = "All files (*.*)|*.*"; // Customize filter as needed

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string selectedFile = openFileDialog.FileName;
            Console.WriteLine("Selected file: " + selectedFile);
            string readText = File.ReadAllText(selectedFile);
            Console.WriteLine(readText);

            string fileType = Path.GetExtension(selectedFile);
            Console.WriteLine("File type: " + fileType);

            string newText = "";
            string[] readBy = readText.Split('\n');
            bool lastLineEmpty = true;//Intial true incase first line is empty
            bool isFirst = true;
            int tabSpacing = 0;
            foreach(string line in readBy){
                Console.WriteLine($"'{line}'"); // helps debugging
                if (string.IsNullOrWhiteSpace(line.Trim()))
                {
                    if(lastLineEmpty){
                        Console.WriteLine("Removing extra line");
                        continue;
                    }else{
                        lastLineEmpty = true;
                        newText += '\n';
                        continue;
                    }
                }else if(isFirst)
                {
                    newText += line.TrimEnd();/*Need no spaces at the end*/
                    lastLineEmpty = false;
                    isFirst = false;
                    continue;
                }
                newText += '\n'/*Add new line here since no need to add at end of line, as may add extra*/ + line.TrimEnd()/*Need no spaces at the end*/;
                lastLineEmpty = false;
            }
            Console.WriteLine("Final");
            Console.WriteLine(newText);
            //Override file with new text
            File.WriteAllText(selectedFile, newText);
        }
        else
        {
            Console.WriteLine("No file selected.");
        }
    }
}
