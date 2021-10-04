# Sharing Inspector 1.1

Sharing Inspector is a tool that making possible to quickly inspect who have access to selected shared folders. It is detecting assigned security groups and displaying all member users for local and domain groups. It also checking if user account is still enabled or not. The result of inspection is presented in csv format, which are very convenient to analyse, for instance in Excel. XML export is also available if needed.

<br /><br />

**QUICK START**


1) Select **Browse** button to choose folders that you want to inspect. You can select many at once or if you have folder paths, just paste it into text box above the button, Please remember to separate paths by semicolon.
2) Select **Inspect** to start inspection and wait for results.
3) At the end you can save results int .csv or .xml file. The file will appear in program location.

<br /><br />

**WHAT WILL BE IN OUTPUT?**

The program is listing users that have access to folder by:
- AD groups
- Local groups
- Domain account set explicitly
- Local account set explicitly


Example:

|Folder|GroupName|Type|Member|SamAccountName|Status|Fullpath
| ------------- | ------------- | ------------- | ------------- |------------- |------------- |------------- |
HR Data|HR_Data_Read_Write|Domain|John Doe|JDoe|Enabled|D:\Share\HR Data
HR Data|HR_Data_Read_Write|Domain|Jane Roe|JRoe|Enabled|D:\Share\HR Data
HR Data|User account|Local|Ron Phillips|RPhillips|Enabled|D:\Share\HR Data
HR Data|BUILTIN\Administrators|Local|Administrator|Unknown|Not available|D:\Share\HR Data
Office Stuff|Domain Users|Domain|All Domain Users|Unknown|Not available|D:\Share\Office Stuff
Office Stuff|BUILTIN\Administrators|Local|Administrator|Unknown|Not available|D:\Share\Office Stuff

* Folder - name of inspected folder
* Member - First name + last name of the user in AD
* SamAccountName - name of user account in AD
* Status - the status of user account in AD
* SamAccountName - Full path to the folder on local drive.

The program is showing all members from nested groups in AD but is not presenting nested group names in output!

The program is NOT showing members of nested LOCAL groups, only names of these groups.

**All Domain Users** - If program find out that something similar to *Domain Users* group is assigned to some shared folder then will not display all your domain users in output. There will be only one single record about that all domain users have access to folder.

**User account** points to user that permissions has not been assigned by the AD or local security group. Usually it's bad practice.

<br /><br />

**OTHER OPTIONS.**


**Include all subfolders** – if subfolders have ONLY inharited rules then their names will not be displayed in output at all. It will be presented only if at least one explicit rule exist or inharitance has been disabled. <br />

It’s recommended to leave below options unchanged, unless you are 100% sure what you want to achive.<br /><br />
**Domain** – Info about detected domain name.<br />
**Prefix** – it’s a domain prefix usually added to name of AD secutirty groups. The program is using this field to sanitize group name before querying domain about group members.<br />
**Container** – the AD location which will be used for cheking groups membership and accounts status.<br />

<br /><br />

**ADDITIONAL INFO**


This program has been tested on large real-life production servers. 
For sure there will be some exceptions in different environments. 
To make this program better please report them to me at: mateoo1@10g.pl


Program layout:  

![sharing_inspector_layout](https://user-images.githubusercontent.com/32539815/135668252-d5df0c95-0d2f-4fa3-9435-ed8c90ec5cd9.jpg)


**RELEASE NOTES**

v. 1.1

1) Fix for folder not found exception. Paths that not exist will not stop program from now. Not existing paths will be displayed at the end of the output.
2) Delimiter changed from "," to ";".
3) Notification about that program can't connect with domain and that is not launched as Administrator from now will be displayed in UI not in message boxs on startup.
