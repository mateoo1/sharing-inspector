# Sharing Inspector 1.0

Sharing Inspector is a tool that making possible to quickly inspect which domain users have access to selected shared folders. It is detecting assigned security groups and displaying all member users. It also checking if user account is still enabled or not. The result of inspection is presented in csv or xml format, which are very convenient to analyse for instance in Excel.

**How to use Sharing Inspector.**

1) Select **Browse** button to choose folders that you want to inspect. You can select many at once or if you have folder paths, just paste it into text box above the button, Please remember to separate paths by semicolon.
2) Select **Inspect** to start inspection and wait for results.
3) At the end you can save results int .csv or.xml file. The file will appear in program location.

**What will be in output?**

Example:

|Folder|FullName|AdGroupName|SamAccountName|Status|Fullpath
| ------------- | ------------- | ------------- |------------- |------------- |------------- |
HR Data|John Doe|HR_Data_Read_Write|JDoe|Enabled|D:\Share\HR Data
HR Data|Jane Roe|HR_Data_Read_Write|JRoe|Enabled|D:\Share\HR Data

* Folder - name of inspected folder
* FullName - First name + last name of the user in AD
* SamAccountName - name of user aacount in AD
* Status - the status of user account in AD
* SamAccountName - Full path to the folder on local drive.

**Include all subfolders** – this option makes you able to look in every subfolder of selected folders. Using this option will consume more time, as the program is drilling subfolders to the end. 

It’s recommended to leave below options unchanged, unless you are 100% sure what you want to achive.
- **Domain** – Info about detected domain name.
- **Prefix** – it’s a domain prefix usually added to name of AD secutirty groups. The program is using this field to sanitize group name before querying domain about group members.
- **Container** – the AD location which will be used for cheking groups membership and accounts status.

**Additional information**

> If program find out that Domain Users group is assigned to some shared folder then will not display all your domain users in output. There will be only one single record about that Domain Users has been assigned to folder.


This program has been tested on large real-life production servers. 
For sure there will be some exceptions in different environments. 
To make this program better please report them to me at: mateoo1@10g.pl


Program layout:  

![sharing-inspector](https://user-images.githubusercontent.com/32539815/132106021-e43863ce-8621-4d28-8672-aa4ee6ef3b7f.jpg)
