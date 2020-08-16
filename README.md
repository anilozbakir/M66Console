# M66Console
## M66 module serial port console for PC

uses [M66FileIO](https://github.com/anilozbakir/M66FileIO) for listing/reading/writing/deleting file. 
you can also define default directories for downloading/uploading of files in the module.
configuration file is at `M66Console/M66Console/Resources/` which includes default comport baudrate and download/upload directories.

works like a console with short cut keys as commands

## Basic Commands

<H3>ListFiles</H3> <H5 >Shift+Ctrl+L</H5>  
    Reads all the file list in the module and saves them as a file list
    if an add/removal of a file is done in the module this command must be processed again
    

<H3>ReadFiles</H3> <H5 >Shift+Ctrl+R</H5>  
    Read file in the list using numbers in the headers
    

<H3>WriteFile</H3> <H5 >Shift+Ctrl+W</H5>  

    Enter the new name of the file and then write the contents when promted. use Ctrl+z to end tyoing data

<H3>DeleteFile</H3> <H5 >Shift+Ctrl+D</H5>  


    Select the File from list and Delete.use List Command to refresh the list
    
<H3>Append File</H3> <H5 >Shift+Ctrl+A</H5>  

    Select file from appering list
    Write content and use Ctrl+z to end typing data

### Upload File Shift+Ctrl+P

    The default upload directory files are read when the program is first run .
    they come as a list and the file is copied to module with the same name and content.
    so the user can use an ordinary word processor to create files
    
### Download File Shift+Ctrl+T
 The files in the module are read with list. the selected file is copied to default 
 download directory with the same name and content.
 
    
