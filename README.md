# FluentFlyout  
<img src="https://github.com/user-attachments/assets/13615f64-c374-4ca3-a274-8842bedf183e" width="58">

<a href="https://apps.microsoft.com/detail/9N45NSM4TNBP?mode=direct">
	<img src="https://get.microsoft.com/images/en-us%20dark.svg" width="200"/>
</a>


FluentFlyout is a simple and modern audio flyout for Windows, built with Fluent 2 Design principles.  
The UI seemingly blends in with Windows 10/11, providing you an uninterrupted, clean, and native-like experience when controlling your media.  

FluentFlyout features smooth animations, supports both light and dark modes, blends with your system's color theme, and includes multiple layout positions while providing media controls and information in a nice and modern looking popup flyout, just above the native volume flyout.

## Features ✨
- Native Windows-like design
- Uses Fluent 2 components
- Utilises Windows Mica blur
- Supports Light and Dark mode
- Matches your device color theme
- Smooth animations
- Customizable flyout positions
- Listens to both volume and media inputs
- Sits unobtrusively in system tray
- _Kinda_ lightweight and open-source
- **Audio flyout: Displays Cover, Title, Artist and media controls**  

## Audio flyout 🎵
> ☀️ Light Mode & Dark Mode 🌘
![FluentFlyoutDemo1](https://github.com/user-attachments/assets/aeeb7b9c-0a90-41d7-aa99-5796796ad29e)

## How to install
### Using Microsoft Store (Recommended)
<a href="https://apps.microsoft.com/detail/9N45NSM4TNBP?mode=direct">
	<img src="https://get.microsoft.com/images/en-us%20dark.svg" width="300"/>
</a>

> Looking for FluentFlyout Settings? You can access it by clicking the system tray icon
### Using .msixbundle installer
> [!Important]
> It's highly recommended to download FluentFlyout from MS store, as it's more convenient and provides auto updates
1. Go to the [latest release](https://github.com/unchihugo/FluentFlyout/releases/latest) page
2. Download the **"*.cer"** file *(I'm too poor to afford a real certificate)*
3. Open the certificate and press **"Install Certificate..."**
4. On the Certificate Import Wizard, select **"Local Machine"**, press **"Next"** and grant Admin Access
5. Select **"Place all certificates in the following store"**, then **"Browse..."**, choose **"Trusted Root Certification Authorities"** and **"OK"**
6. Finally, press **"Next"** and then **"Finish"**. It might ask you to confirm, press **Yes**
7. Download **"FluentFlyout_*.msixbundle"**
8. The App Installer will pop up, press **"Install"**, or **"Update"** if you've installed FluentFlyout before
9. done! try playing music and use your media or volume keys

## Upcoming features 📝
- [ ] More animations
- [x] Settings
- [x] Editable flyout timeout
- [ ] Implement compact layout
- [x] Remove Windows Forms dependency
- [ ] Add more media controls (repeat, shuffle, seek slider)
### Issues
- FluentFlyout might interfere with certain apps/games in **Fullscreen**, try setting the program's window mode to **Borderless Fullscreen** for now
- Windows 10 UI might not look as expected

## Contributing 💖
Please feel free to contribute in any way you can!

## Credits 🙌
[Hugo Li](https://unchihugo.github.io) - Original Developer and Microsoft Store Publisher
### Dependencies
- [Dubya.WindowsMediaController](https://github.com/DubyaDude/WindowsMediaController)
- [H.NotifyIcon.Wpf](https://github.com/HavenDV/H.NotifyIcon)
- [MicaWPF](https://github.com/Simnico99/MicaWPF)
- [WPF-UI](https://github.com/lepoco/wpfui)
