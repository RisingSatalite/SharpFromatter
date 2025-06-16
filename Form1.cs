namespace SharpFromatter;

using System.IO;
using System.Collections;

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

			List<string> Unsupported = new List<string> { ".img", ".svg", ".png" };
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
			Stack<int> indentSpacingCounter = new Stack<int>();//Used for indent based scripts
			indentSpacingCounter.Push(0);

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
				List<string> SQLBased = new List<string> { ".sql" };

				if (BraceBased.Contains(fileType))
				{
					int countOpenBracket = line.Count(c => c == '{');
					int countCloseBracket = line.Count(c => c == '}');
					bool offSetForClosingBracket = false;
					if (countCloseBracket == 1 && countOpenBracket == 1)
					{
						if (line.IndexOf('{') > line.IndexOf('}'))
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

					if (offSetForClosingBracket)
					{
						indentSpacing += 1;
					}

					//Change offset for next line
					indentSpacing += countOpenBracket - countCloseBracket;
				}
				else if (IndentationBased.Contains(fileType))
				{
					//Is it a full comment
					string simpleLine = line.Trim();
					if (simpleLine[0] == '#')
					{
						newText += '\n' + new string('\t', Math.Max(indentSpacing, 0)) + line.Trim();
						continue;
					}

					//Detect indentation
					int whitespaceCount = 0;
					foreach (char c in line)
					{
						if (char.IsWhiteSpace(c))
							whitespaceCount++;
						else
							break;
					}

					if (whitespaceCount == indentSpacingCounter.Peek())
					{
						Console.WriteLine("Same block");
						//No need to change indentSpacingCounter as equal right now
						newText += '\n' + new string('\t', Math.Max(indentSpacing, 0)) + line.Trim();
					}
					else if (whitespaceCount > indentSpacingCounter.Peek())
					{
						Console.WriteLine("New block");
						indentSpacing += 1;
						indentSpacingCounter.Push(whitespaceCount);
						newText += '\n' + new string('\t', Math.Max(indentSpacing, 0)) + line.Trim();
					}
					else if (whitespaceCount < indentSpacingCounter.Peek())
					{
						Console.WriteLine("Returning to old block");
						while (true)
						{
							indentSpacing -= 1;
							indentSpacingCounter.Pop();
							if (whitespaceCount == indentSpacingCounter.Peek())
							{
								break;
							}
							else if (whitespaceCount > indentSpacingCounter.Peek())
							{
								//Incase of some sort of issue where indent is not seen before, this is a sort of failsafe
								Console.WriteLine("Indent issue detected, failsafe activated");
								indentSpacingCounter.Push(whitespaceCount);
								indentSpacing += 1;
								break;
							}
						}
						newText += '\n' + new string('\t', Math.Max(indentSpacing, 0)) + line.Trim();
					}
				}
				else if (SQLBased.Contains(fileType))
				{
					string processedLine = line.Trim();
					string[] sqlKeywords = { "SELECT", "FROM", "WHERE", "GROUP BY", "ORDER BY", "HAVING", "JOIN", "INNER JOIN", 
											"LEFT JOIN", "RIGHT JOIN", "ON", "AND", "OR", "INSERT", "UPDATE", "DELETE", "VALUES", 
											"SET", "CREATE", "PROCEDURE", "INTO", "IN", "OUT", "INOUT", "AS", "BEGIN", "END" };

					// Uppercase all SQL keywords
					foreach (string keyword in sqlKeywords)
					{
						processedLine = System.Text.RegularExpressions.Regex.Replace(
							processedLine,
							$@"\b{keyword}\b",
							match => match.Value.ToUpper(),
							System.Text.RegularExpressions.RegexOptions.IgnoreCase);
					}

					// Handle CREATE PROCEDURE param lists
					if (processedLine.Contains("CREATE") && processedLine.Contains("PROCEDURE"))
					{
						// Detect start of param list
						int paramStart = processedLine.IndexOf('(');
						if (paramStart >= 0)
						{
							string beforeParams = processedLine.Substring(0, paramStart + 1);
							string paramsAndAfter = processedLine.Substring(paramStart + 1);

							// Split params by ',' to add new lines and tabs
							string[] paramParts = paramsAndAfter.Split(',');
							string formattedParams = "";

							for (int i = 0; i < paramParts.Length; i++)
							{
								string param = paramParts[i].Trim();
								// Add closing bracket ')' handling if present
								if (param.Contains(")"))
								{
									int closeParenIndex = param.IndexOf(')');
									string actualParam = param.Substring(0, closeParenIndex).Trim();
									formattedParams += "\n\t" + actualParam + "\n)";
								}
								else
								{
									formattedParams += "\n\t" + param + ",";
								}
							}

							// Remove last comma if present
							if (formattedParams.EndsWith(","))
								formattedParams = formattedParams.Substring(0, formattedParams.Length - 1);

							processedLine = beforeParams + formattedParams;
						}
					}

					newText += "\n" + new string('\t', Math.Max(indentSpacing, 0)) + processedLine;
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
