# SE-TextureConverter

Commandline-Tool which converts Space engineers textures to an 3dsmax/Blender readable format.
Includes:
  - Multithreading
  - "Fix" for some texures whose RGB was deleted after conversion (is hacky but meh)
  - excludes skins folder, these aren't moddable anyway
  - "-updateonly" command to only convert textures which are new/chnaged


 ✓heavily borrowed code from that UI based thingy: https://github.com/AtlasTheProto/SETextureConverter

## Usage
```
TextureConverter.exe -GamePath -Outpath -Textureroot -Textureroot2 -Textureroot3 -Texturerootn... -updateonly
```

**GamePath**: Space Engineers game folder.
 - Not .exe folder by main root folder of Space Engineers!


**Outpath**: Where the Textures shall be saved to.#
  - Any folder you want to export the new images to.

**Textureroot**: relative start Path for the Textures. 
  - (sub folders are automatically included) 
  - (relative to GamePath) 
  - Can use as many as you want.


 
 
 
