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

			//Not able to format
			if(fileType == ".png" || fileType == ".svg" || fileType == ".img"){
				Console.WriteLine("File type not supported");
				return;
			}

			//Varibles
			string newText = "";
			string[] readBy = readText.Split('\n');
			bool lastLineEmpty = true;//Intial true incase first line is empty
			bool isFirst = true;
			int indentSpacing = 0;
			bool commented = false;

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

				if(fileType == ".cs"){
					//Added the line

				int countCloseBracket = line.Count(c => c == '}');

				bool offSetForClosingBracket = false;//Incase the below is true
				if(countCloseBracket>0){
					offSetForClosingBracket = true;
					indentSpacing -= 1;
				}
				newText += '\n' + new string('\t', Math.Max(indentSpacing, 0)) + line.Trim();

				int countOpenBracket = line.Count(c => c == '{');

					if(offSetForClosingBracket){
						indentSpacing += 1;
					}
					indentSpacing += countOpenBracket - countCloseBracket;
				}else{
					newText += '\n'/*Add new line here since no need to add at end of line, as may add extra*/ + line.TrimEnd()/*Need no spaces at the end*/;
				}
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
