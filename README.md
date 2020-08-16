# M66Console
## M66 module serial port console

uses [M66FileIO](https://github.com/anilozbakir/M66FileIO) for listing/reading/writing/deleting file. 
you can also define default directories for downloading/uploading of files in the module.
configuration file is at `M66Console/M66Console/Resources/` which includes default comport baudrate and download/upload directories.

works like a console with short cut keys as commands

## Basic Commands

### ListFiles Shift+Ctrl+L  
    Reads all the file list in the module and saves them as a file list
    if an add/removal of a file is done in the module this command must be processed again
    
### ReadFiles Shift+Ctrl+R
  
    Read file in the list using numbers in the headers
    
### WriteFile Shift+Ctrl+W

    Enter the new name of the file and then write the contents when promoted

### Upload File Shift+Ctrl+P

    The default upload directory files are read when the program is first run .they come as a list and the
    file is copied to module with the same name and content.so the user can use an ordinary word processor to create files
    
### Download File Shift+Ctrl+
 The files in the module are read with list. the selected file is copied to default download directory with the same name and content.
 
    
