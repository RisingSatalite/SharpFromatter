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

            string newText = "";
            string[] readBy = readText.Split('\n');
            bool lastLineEmpty = false;
            foreach(string line in readBy){
                Console.WriteLine(line);
                if(line == null || line == ""){
                    if(lastLineEmpty){
                        continue;
                    }else{
                        lastLineEmpty = true;
                    }
                }
                newText += line + '\n';
            }
            Console.WriteLine("Final");
            Console.WriteLine(newText);
        }
        else
        {
            Console.WriteLine("No file selected.");
        }
    }
}
