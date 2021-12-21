# SE-TextureConverter

Commandline-Tool which converts Space engineers textures to an 3dsmax/Blender readable format.
Includes:
  - Multithreading
  - "Fix" for some texures whose RGB was deleted after conversion (is hacky but meh)


## Usage
```
TextureConverter.exe -GamePath -Outpath -Textureroot -Textureroot2 -Textureroot3 -Texturerootn...
```

**GamePath**: Space Engineers game folder.
 - Not .exe folder by main root folder of Space Engineers!


**Outpath**: Where the Textures shall be saved to.#
  - Any folder you want to export the new images to.

**Textureroot**: relative start Path for the Textures. 
  - (sub folders are automatically included) 
  - (relative to GamePath) 
  - Can use as many as you want.
 
 
 