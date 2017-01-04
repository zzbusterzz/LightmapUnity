# LightmapUnity
1) Build lightmap with required objects in scene

2) Duplicate the generated lightmaps

3) Goto Windows->BeastHelperPlus

4) Click on "Read Current Maps"

5) Click on "Store Maps info to Disk"

6) Do the above procedure for all different cases of scene partitioned lightmaps

7) Make sure to clear lightmaps before baking new partition

8) After Saving lightmaps now it is time to load them

9) Click on BeasHelperplus from step 3

10) Click "Load maps info from Disk"->Click on First lightmap->Select Replace(Do not select merge as this is our start point of lightmap merging)

11) Set lightmaps repective files in editor

12) Click Set Actual maps in Beast

13) Now repeat the procedure but only instead of repleace you will have to merge

#Note: 
While loading map info turn off the baked lights as it has been noticed the light becomes too bright
