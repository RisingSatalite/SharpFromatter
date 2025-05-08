namespace SharpFromatter;

using System.IO;

public partial class Form1 : Form
{
	public Form1()
	{
		InitializeComponent();
		Button rerunButton = new Button();
		rerunButton.Text = "Select file to Reformat";
		rerunButton.Location = new Point(10, 10); // adjust position
		rerunButton.Size = new Size(200, 100); // Width = 200px, Height = 60px
		rerunButton.Click += (sender, e) => RunFormatter();
		this.Controls.Add(rerunButton);
	}

	private void RunFormatter()
	{
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

			if (fileType == ".png" || fileType == ".svg" || fileType == ".img")
			{
				Console.WriteLine("File type not supported");
				return;
			}

			string newText = "";
			string[] readBy = readText.Split('\n');
			bool lastLineEmpty = true;
			bool isFirst = true;
			int indentSpacing = 0;

			foreach (string line in readBy)
			{
				Console.WriteLine($"'{line}'");
				if (string.IsNullOrWhiteSpace(line.Trim()))
				{
					if (lastLineEmpty)
					{
						Console.WriteLine("Removing extra line");
						continue;
					}
					else
					{
						lastLineEmpty = true;
						newText += '\n';
						continue;
					}
				}
				else if (isFirst)
				{
					newText += line.TrimEnd();
					lastLineEmpty = false;
					isFirst = false;
					continue;
				}

				List<string> BraceBased = new List<string> { ".js", "ts", ".cs", ".c", ".cpp", ".java" };
				List<string> IndentationBased = new List<string> { ".py", ".gd" };
				List<string> MarkupBased = new List<string> { ".html", ".jsx" };
				List<string> KeywordBased = new List<string> { ".lua" };
				List<string> ExpressiveBased = new List<string> { ".scheme" };

				if (BraceBased.Contains(fileType))
				{
					int countOpenBracket = line.Count(c => c == '{');
					int countCloseBracket = line.Count(c => c == '}');
					bool offSetForClosingBracket = false;
					if (countCloseBracket == 1 && countOpenBracket == 1){
						if(line.IndexOf('{') > line.IndexOf('}'))
						{
							offSetForClosingBracket = true;
							indentSpacing -= 1;
						}
					}
					else if (countCloseBracket > 0)
					{
						offSetForClosingBracket = true;
						indentSpacing -= 1;
					}

					newText += '\n' + new string('\t', Math.Max(indentSpacing, 0)) + line.Trim();

					if (offSetForClosingBracket){
						indentSpacing += 1;
					}

					indentSpacing += countOpenBracket - countCloseBracket;
				}
				else
				{
					newText += '\n' + line.TrimEnd();
				}
				lastLineEmpty = false;
			}

			Console.WriteLine("Final");
			Console.WriteLine(newText);
			File.WriteAllText(selectedFile, newText);
		}
		else
		{
			Console.WriteLine("No file selected.");
		}
	}

}
