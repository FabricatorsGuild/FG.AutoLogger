using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using FG.Diagnostics.AutoLogger.Model;

namespace FG.Diagnostics.AutoLogger.Tool
{
    public class VersionChecker : BaseWithLogging
    {
        private readonly Regex _versionParserRegEx = new Regex(@"(?'major'\d+).(?'minor'\d+).(?'revision'\d+).(?'build'\d+)(?'prerelease'\-[a-zA-Z0-9]+){0,1}", RegexOptions.Compiled);

        public void CheckVersion()
        {
            var version = this.GetType().Assembly.GetName().Version;
            var githubLatestReleaseTitle = DownloadHtmlTitleString("https://github.com/FabricatorsGuild/FG.AutoLogger/releases/latest");

            var onlineVersion = GetVersionFromString(githubLatestReleaseTitle);


            var versionComparison = version.CompareTo(onlineVersion);
            if( versionComparison < 0)
            {
                LogError($"Latest version is {onlineVersion} but this tool is running {version}, consider updating the tool or disable version checking with the -n option of this tool");
                throw new NotSupportedException($"Latest version is {onlineVersion} but this tool is running {version}, consider updating the tool or disable version checking with the -n option of this tool");

            }
            else if (versionComparison > 0)
            {
                LogMessage(@"You are currently running a verson later than the latest stable release");
            }
        }

        private Version GetVersionFromString(string stringContainingVersion)
        {
            var match = _versionParserRegEx.Match(stringContainingVersion);
            if (match.Success)
            {
                var major = int.Parse(match.Groups["major"].Value);
                var minor = int.Parse(match.Groups["minor"].Value);
                var revision = int.Parse(match.Groups["revision"].Value);
                var build = int.Parse(match.Groups["build"].Value);

                var prerelease = match.Groups["prerelease"].Value;

                return new Version(major, minor, revision, build);
            }

            return null;
        }

        private string DownloadHtmlTitleString(string uri)
        {
            var title = "";
            try
            {
                var request = (WebRequest.Create(uri) as HttpWebRequest);

                var response = request?.GetResponse() as HttpWebResponse;
                if (response == null)
                {
                    LogWarning($"Failed to download the Uri {uri}");
                    return null;
                }

                using (var stream = response.GetResponseStream())
                {
                    // compiled regex to check for <title></title> block
                    var titleCheck = new Regex(@"<title>\s*(.+?)\s*</title>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    var bytesToRead = 8092;
                    var buffer = new byte[bytesToRead];
                    var contents = "";
                    var length = 0;
                    while (stream != null && (length = stream.Read(buffer, 0, bytesToRead)) > 0)
                    {
                        // convert the byte-array to a string and add it to the rest of the
                        // contents that have been downloaded so far
                        contents += Encoding.UTF8.GetString(buffer, 0, length);

                        var m = titleCheck.Match(contents);
                        if (m.Success)
                        {
                            // we found a <title></title> match =]
                            title = m.Groups[1].Value.ToString();
                            break;
                        }
                        else if (contents.Contains("</head>"))
                        {
                            // reached end of head-block; no title found =[
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to download the Uri {uri} - {ex.Message}");
                return null;
            }

            return title;
        }
    }
}