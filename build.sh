# Get the device UDID from XCode Devices window
# Do a chmod +x build.sh to give the file permission
# Execute like this:
#  ./build.sh

# Find and delete .DS_Store files
find . -name ".DS_Store" -type f -delete

# Find and delete .AppleDouble files
find . -name ".AppleDouble" -type d -exec rm -rf {} +

# Find and delete files starting with ._ (e.g., ._example.txt)
find . -name "._*" -type f -delete

rm -rf bin
rm -rf obj

#dotnet build -t:Run -f net7.0-ios -p:_DeviceName=:v2:udid=0D52415F-8501-4DCF-81F1-84894971D9EB