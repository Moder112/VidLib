# VidLib
A C# clone of youtube-dl designed to download VidLii videos and metadata

A compiled version available here:
https://github.com/Moder112/VidLib/releases/tag/VidLib

I created it to resolve the issue of archiving siivagunner-type channels on vidlii.
I might add compatibility for bitview or whatever it's called later.
Anyway the program is a ripoff of youtube-dl and basically made clones of some commands that are useful for archival.

Here are the arguments the program will recognise:
```
-help - prints help
-HQ - Downloads the 720p version of the video if possible
-get-thumb - Downloads the thumbnail
-save-metadata - Saves the json metadata of the video
-no-video - skips downloading the video file (useful for updating metadata)

-path <file> - A mandatory argument, followed by the path to the file, path takes special arguments to generate the file location
  @[url] - video id
  @[file] - cdn server file id
  @[hd] - Hd video status *UNUSED*
  @[title] - video title
  @[category] - Video Category
  @[uploaded_by] - uploader
  @[uploaded_on] - upload date
  @[ext] - Extension (MANDATORY)
  
For example this is a valid path
C:\Users\me\Documents\Vidlii\@[uploaded_by]\@[url]\@[title]@[ext]

-archive <filename> - saves the downloaded video id to a file so that you don't download it again.


Use:

Vidlib.exe [misc arguments] [-archive <file>] -path <location> <Link/path to file with links> 
  
```

TODO
======
probably gonna add some kind of auto downloader for updates to complete the youtube-dl ripoff checklist









i have no idea why the hell is this marked as an html project, there's only one html file and it's really small, wtf github
