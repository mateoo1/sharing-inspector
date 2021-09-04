# Sharing Inspector 1.0

Sharing Inspector is a tool that making possible to quickly inspect which domain users have access to selected shared folders. It is detecting assigned security groups and displaying all member users. It also checking if user account is still enabled or not. The result of inspection is presented in csv or xml format, which are very convenient to analyse for instance in Excel.

**How to use Sharing Inspector.**

1. Select **Browse** button to choose folders that you want to inspect. You can select many at once or if you have folder paths, just paste it into text box above the button, Please remember to separate paths by semicolon.
2. Select **Inspect** to start inspection and wait for results.
3. At the end you can save results int .csv or.xml file. The file will appear in program location.

**Include all subfolders** – this option makes you able to look in every subfolder of selected folders. Using this option will consume more time, as the program is drilling subfolders to the end. 

It’s recommended to leave below options unchanged, unless you are 100% sure what you want to achive.
- **Domain** – Info about detected domain name.
- **Prefix** – it’s a domain prefix usually added to name of AD secutirty groups. The program is using this field to sanitize group name before querying domain about group members.
- **Container** – the AD location which will be used for cheking groups membership and accounts status. 