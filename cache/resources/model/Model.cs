using OpenRSCS.utils;

namespace OpenRSCS.cache.resources.model {
    public class Model {

        public readonly bool aBool1775 = false;
        public readonly sbyte[] aByteArray1735;
        public readonly int anInt1745;
        public readonly short[] aShortArray1750;
        public readonly short[] aShortArray1751;
        public readonly short[] aShortArray1754;
        public readonly short[] aShortArray1767;
        public readonly short[] aShortArray1768;
        public readonly bool newVersion;
        public readonly sbyte priority;
        public readonly short[] texPrimaryColor;
        public readonly int[] vertexSkins;
        public short aShort1764;
        public short aShort1766;
        public sbyte[] faceAlphas;
        public short[] faceColor;
        public int faceCount;
        public sbyte[] facePriorities;
        public short[] faceTextures;
        public sbyte[] faceType;
        public int[] skinValues;
        public short[] texTriX;
        public short[] texTriY;
        public short[] texTriZ;
        public sbyte[] texturePos;
        public sbyte[] textureRenderTypes;
        public int[] triangleX;
        public int[] triangleY;
        public int[] triangleZ;
        public int vertexCount;
        public int[] vertexX;
        public int[] vertexY;
        public int[] vertexZ;

        public Model(ByteBuffer buf) {
            int initPos = (int)buf.Position;
            buf.Position = buf.Length - 2;
            newVersion = buf.getShort() == -1;
            buf.Position = initPos;
            if(newVersion) {
                ByteBuffer var2 = buf.clone();
                ByteBuffer var9 = buf.clone();
                ByteBuffer var33 = buf.clone();
                ByteBuffer var49 = buf.clone();
                ByteBuffer var7 = buf.clone();
                ByteBuffer var8 = buf.clone();
                ByteBuffer var46 = buf.clone();

                var2.position((int)(buf.Length - 23));

                int numVertices = var2.getShort() & 0xFFFF;
                int numTriangles = var2.getShort() & 0xFFFF;
                int numTextureTriangles = var2.getSByte() & 0xFF;
                int var13 = var2.getSByte() & 0xFF;
                int modelPriority = var2.getSByte() & 0xFF;
                int var16 = var2.getSByte() & 0xFF;
                int var4 = var2.getSByte() & 0xFF;
                int texture = var2.getSByte() & 0xFF;
                int modelSkins = var2.getSByte() & 0xFF;
                int var20 = var2.getShort() & 0xFFFF;
                int var21 = var2.getShort() & 0xFFFF;
                int var22 = var2.getShort() & 0xFFFF;
                int var23 = var2.getShort() & 0xFFFF;
                int var24 = var2.getShort() & 0xFFFF;

                int textureCount = 0;
                int var3 = 0;
                int var26 = 0;
                int var5;
                if(numTextureTriangles > 0) {
                    textureRenderTypes = new sbyte[numTextureTriangles];
                    var2.position(0);

                    for(var5 = 0; var5 < numTextureTriangles; var5++) {
                        sbyte type = textureRenderTypes[var5] = var2.getSByte();
                        if(type == 0) {
                            ++textureCount;
                        }

                        if(type >= 1 && type <= 3) {
                            ++var3;
                        }

                        if(type == 2) {
                            ++var26;
                        }
                    }
                }

                var5 = numTextureTriangles + numVertices;
                int var561 = var5;
                if(var13 == 1) {
                    var5 += numTriangles;
                }

                int var35 = var5;
                var5 += numTriangles;
                int var56 = var5;
                if(modelPriority == 255) {
                    var5 += numTriangles;
                }

                int var37 = var5;
                if(var4 == 1) {
                    var5 += numTriangles;
                }

                int var48 = var5;
                if(modelSkins == 1) {
                    var5 += numVertices;
                }

                int var39 = var5;
                if(var16 == 1) {
                    var5 += numTriangles;
                }

                int var40 = var5;
                var5 += var23;
                int var57 = var5;
                if(texture == 1) {
                    var5 += numTriangles*2;
                }

                int var42 = var5;
                var5 += var24;
                int var50 = var5;
                var5 += numTriangles*2;
                int var10 = var5;
                var5 += var20;
                int var51 = var5;
                var5 += var21;
                int var6 = var5;
                var5 += var22;
                int var43 = var5;
                var5 += textureCount*6;
                int var52 = var5;
                var5 += var3*6;
                int var44 = var5;
                var5 += var3*6;
                int var54 = var5;
                var5 += var3*2;
                int var45 = var5;
                var5 += var3;
                int var47 = var5;
                var5 += var3*2 + var26*2;

                vertexCount = numVertices;
                faceCount = numTriangles;
                anInt1745 = numTextureTriangles;
                vertexX = new int[numVertices];
                vertexY = new int[numVertices];
                vertexZ = new int[numVertices];
                triangleX = new int[numTriangles];
                triangleY = new int[numTriangles];
                triangleZ = new int[numTriangles];

                if(modelSkins == 1) {
                    vertexSkins = new int[numVertices];
                }

                if(var13 == 1) {
                    faceType = new sbyte[numTriangles];
                }

                if(modelPriority == 255) {
                    facePriorities = new sbyte[numTriangles];
                } else {
                    priority = (sbyte)modelPriority;
                }

                if(var16 == 1) {
                    faceAlphas = new sbyte[numTriangles];
                }

                if(var4 == 1) {
                    skinValues = new int[numTriangles];
                }

                if(texture == 1) {
                    faceTextures = new short[numTriangles];
                }

                if(texture == 1 && numTextureTriangles > 0) {
                    texturePos = new sbyte[numTriangles];
                }

                faceColor = new short[numTriangles];
                if(numTextureTriangles > 0) {
                    texTriX = new short[numTextureTriangles];
                    texTriY = new short[numTextureTriangles];
                    texTriZ = new short[numTextureTriangles];
                    if(var3 > 0) {
                        aShortArray1750 = new short[var3];
                        aShortArray1751 = new short[var3];
                        aShortArray1767 = new short[var3];
                        aShortArray1768 = new short[var3];
                        aByteArray1735 = new sbyte[var3];
                        aShortArray1754 = new short[var3];
                    }

                    if(var26 > 0) {
                        texPrimaryColor = new short[var26];
                    }
                }

                var2.position(numTextureTriangles);
                var9.position(var10);
                var33.position(var51);
                var49.position(var6);
                var7.position(var48);

                int var53 = 0;
                int var28 = 0;
                int var34 = 0;

                int vertetxOffsetZ;
                int var15;
                int vertextOffsetY;
                int flags;
                int vertextOffsetX;
                for(var15 = 0; var15 < numVertices; var15++) {
                    flags = var2.getSByte() & 0xFF;
                    vertextOffsetX = 0;
                    if((flags & 0x1) != 0) {
                        vertextOffsetX = var9.getSmartA();
                    }

                    vertextOffsetY = 0;
                    if((flags & 0x2) != 0) {
                        vertextOffsetY = var33.getSmartA();
                    }

                    vertetxOffsetZ = 0;
                    if((flags & 0x4) != 0) {
                        vertetxOffsetZ = var49.getSmartA();
                    }

                    vertexX[var15] = var53 + vertextOffsetX;
                    vertexY[var15] = var28 + vertextOffsetY;
                    vertexZ[var15] = var34 + vertetxOffsetZ;
                    var53 = vertexX[var15];
                    var28 = vertexY[var15];
                    var34 = vertexZ[var15];
                    if(modelSkins == 1) {
                        vertexSkins[var15] = var7.getSByte() & 0xFF;
                    }
                }

                var2.position(var50);
                var9.position(var561);
                var33.position(var56);
                var49.position(var39);
                var7.position(var37);
                var8.position(var57);
                var46.position(var42);

                for(var15 = 0; var15 < numTriangles; var15++) {
                    faceColor[var15] = (short)(var2.getShort() & 0xFFFF);
                    if(var13 == 1) {
                        faceType[var15] = var9.getSByte();
                    }

                    if(modelPriority == 255) {
                        facePriorities[var15] = var33.getSByte();
                    }

                    if(var16 == 1) {
                        faceAlphas[var15] = var49.getSByte();
                    }

                    if(var4 == 1) {
                        skinValues[var15] = var7.getSByte() & 0xFF;
                    }

                    if(texture == 1) {
                        faceTextures[var15] = (short)(var8.getShort() & 0xFFFF - 1);
                    }

                    if(texturePos != null && faceTextures[var15] != -1) {
                        texturePos[var15] = (sbyte)(var46.getSByte() & 0xFF - 1);
                    }
                }

                var2.position(var40);
                var9.position(var35);
                var15 = 0;
                flags = 0;
                vertextOffsetX = 0;
                vertextOffsetY = 0;

                int var32;
                for(vertetxOffsetZ = 0; vertetxOffsetZ < numTriangles; vertetxOffsetZ++) {
                    var32 = var9.getSByte() & 0xFF;
                    if(var32 == 1) {
                        var15 = var2.getSmartA() + vertextOffsetY;
                        flags = var2.getSmartA() + var15;
                        vertextOffsetX = var2.getSmartA() + flags;
                        vertextOffsetY = vertextOffsetX;
                        triangleX[vertetxOffsetZ] = var15;
                        triangleY[vertetxOffsetZ] = flags;
                        triangleZ[vertetxOffsetZ] = vertextOffsetX;
                    }

                    if(var32 == 2) {
                        flags = vertextOffsetX;
                        vertextOffsetX = var2.getSmartA() + vertextOffsetY;
                        vertextOffsetY = vertextOffsetX;
                        triangleX[vertetxOffsetZ] = var15;
                        triangleY[vertetxOffsetZ] = flags;
                        triangleZ[vertetxOffsetZ] = vertextOffsetX;
                    }

                    if(var32 == 3) {
                        var15 = vertextOffsetX;
                        vertextOffsetX = var2.getSmartA() + vertextOffsetY;
                        vertextOffsetY = vertextOffsetX;
                        triangleX[vertetxOffsetZ] = var15;
                        triangleY[vertetxOffsetZ] = flags;
                        triangleZ[vertetxOffsetZ] = vertextOffsetX;
                    }

                    if(var32 == 4) {
                        int var41 = var15;
                        var15 = flags;
                        flags = var41;
                        vertextOffsetX = var2.getSmartA() + vertextOffsetY;
                        vertextOffsetY = vertextOffsetX;
                        triangleX[vertetxOffsetZ] = var15;
                        triangleY[vertetxOffsetZ] = var41;
                        triangleZ[vertetxOffsetZ] = vertextOffsetX;
                    }
                }

                var2.position(var43);
                var9.position(var52);
                var33.position(var44);
                var49.position(var54);
                var7.position(var45);
                var8.position(var47);

                for(vertetxOffsetZ = 0; vertetxOffsetZ < numTextureTriangles; vertetxOffsetZ++) {
                    var32 = textureRenderTypes[vertetxOffsetZ] & 0xff;
                    if(var32 == 0) {
                        texTriX[vertetxOffsetZ] = (short)(var2.getShort() & 0xFFFF);
                        texTriY[vertetxOffsetZ] = (short)(var2.getShort() & 0xFFFF);
                        texTriZ[vertetxOffsetZ] = (short)(var2.getShort() & 0xFFFF);
                    }

                    if(var32 == 1) {
                        texTriX[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        texTriY[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        texTriZ[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        aShortArray1750[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1751[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1767[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1768[vertetxOffsetZ] = (short)(var49.getShort() & 0xFFFF);
                        aByteArray1735[vertetxOffsetZ] = var7.getSByte();
                        aShortArray1754[vertetxOffsetZ] = (short)(var8.getShort() & 0xFFFF);
                    }

                    if(var32 == 2) {
                        texTriX[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        texTriY[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        texTriZ[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        aShortArray1750[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1751[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1767[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1768[vertetxOffsetZ] = (short)(var49.getShort() & 0xFFFF);
                        aByteArray1735[vertetxOffsetZ] = var7.getSByte();
                        aShortArray1754[vertetxOffsetZ] = (short)(var8.getShort() & 0xFFFF);
                        texPrimaryColor[vertetxOffsetZ] = (short)(var8.getShort() & 0xFFFF);
                    }

                    if(var32 == 3) {
                        texTriX[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        texTriY[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        texTriZ[vertetxOffsetZ] = (short)(var9.getShort() & 0xFFFF);
                        aShortArray1750[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1751[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1767[vertetxOffsetZ] = (short)(var33.getShort() & 0xFFFF);
                        aShortArray1768[vertetxOffsetZ] = (short)(var49.getShort() & 0xFFFF);
                        aByteArray1735[vertetxOffsetZ] = var7.getSByte();
                        aShortArray1754[vertetxOffsetZ] = (short)(var8.getShort() & 0xFFFF);
                    }
                }

                var2.position(var5);
                vertetxOffsetZ = var2.getSByte() & 0xFF;
                if(vertetxOffsetZ != 0) {
                    //new Class25();
                    var2.getShort();
                    var2.getShort();
                    var2.getShort();
                    var2.getInt();
                }
            } else {
                bool var2 = false;
                bool var9 = false;

                ByteBuffer var5 = buf.clone();
                ByteBuffer var6 = buf.clone();
                ByteBuffer var7 = buf.clone();
                ByteBuffer var8 = buf.clone();
                ByteBuffer var38 = buf.clone();

                var5.position((int)buf.Length - 18);

                int var10 = var5.getShort() & 0xFFFF;
                int var11 = var5.getShort() & 0xFFFF;
                int var12 = var5.getSByte() & 0xFF;
                int var42 = var5.getSByte() & 0xFF;
                int var15 = var5.getSByte() & 0xFF;
                int var16 = var5.getSByte() & 0xFF;
                int var25 = var5.getSByte() & 0xFF;
                int var17 = var5.getSByte() & 0xFF;
                int var18 = var5.getShort() & 0xFFFF;
                int var19 = var5.getShort() & 0xFFFF;
                var5.getShort();
                int var21 = var5.getShort() & 0xFFFF;

                sbyte var13 = 0;
                int var46 = var13 + var10;
                int var24 = var46;
                var46 += var11;
                int var14 = var46;
                if(var15 == 255) {
                    var46 += var11;
                }

                int var26 = var46;
                if(var25 == 1) {
                    var46 += var11;
                }

                int var22 = var46;
                if(var42 == 1) {
                    var46 += var11;
                }

                int var28 = var46;
                if(var17 == 1) {
                    var46 += var10;
                }

                int var30 = var46;
                if(var16 == 1) {
                    var46 += var11;
                }

                int var39 = var46;
                var46 += var21;
                int var34 = var46;
                var46 += var11*2;
                int var35 = var46;
                var46 += var12*6;
                int var36 = var46;
                var46 += var18;
                int var41 = var46;
                var46 += var19;

                vertexCount = var10;
                faceCount = var11;
                anInt1745 = var12;
                vertexX = new int[var10];
                vertexY = new int[var10];
                vertexZ = new int[var10];
                triangleX = new int[var11];
                triangleY = new int[var11];
                triangleZ = new int[var11];

                if(var12 > 0) {
                    textureRenderTypes = new sbyte[var12];
                    texTriX = new short[var12];
                    texTriY = new short[var12];
                    texTriZ = new short[var12];
                }

                if(var17 == 1) {
                    vertexSkins = new int[var10];
                }

                if(var42 == 1) {
                    faceType = new sbyte[var11];
                    texturePos = new sbyte[var11];
                    faceTextures = new short[var11];
                }

                if(var15 == 255) {
                    facePriorities = new sbyte[var11];
                } else {
                    priority = (sbyte)var15;
                }

                if(var16 == 1) {
                    faceAlphas = new sbyte[var11];
                }

                if(var25 == 1) {
                    skinValues = new int[var11];
                }

                faceColor = new short[var11];

                var5.position(var13);
                var6.position(var36);
                var7.position(var41);
                var8.position(var46);
                var38.position(var28);

                int var29 = 0;
                int var32 = 0;
                int var43 = 0;

                int var3;
                int var4;
                int var23;
                int var27;
                int var33;
                for(var23 = 0; var23 < var10; var23++) {
                    var27 = var5.getSByte() & 0xFF;
                    var4 = 0;
                    if((var27 & 0x1) != 0) {
                        var4 = var6.getSmartA();
                    }

                    var33 = 0;
                    if((var27 & 0x2) != 0) {
                        var33 = var7.getSmartA();
                    }

                    var3 = 0;
                    if((var27 & 0x4) != 0) {
                        var3 = var8.getSmartA();
                    }

                    vertexX[var23] = var29 + var4;
                    vertexY[var23] = var32 + var33;
                    vertexZ[var23] = var43 + var3;
                    var29 = vertexX[var23];
                    var32 = vertexY[var23];
                    var43 = vertexZ[var23];
                    if(var17 == 1) {
                        vertexSkins[var23] = var38.getSByte() & 0xFF;
                    }
                }

                var5.position(var34);
                var6.position(var22);
                var7.position(var14);
                var8.position(var30);
                var38.position(var26);

                for(var23 = 0; var23 < var11; var23++) {
                    faceColor[var23] = (short)(var5.getShort() & 0xFFFF);
                    if(var42 == 1) {
                        var27 = var6.getSByte() & 0xFF;
                        if((var27 & 0x1) == 1) {
                            faceType[var23] = 1;
                            var2 = true;
                        } else {
                            faceType[var23] = 0;
                        }

                        if((var27 & 0x2) == 2) {
                            texturePos[var23] = (sbyte)(var27 >> 2);
                            faceTextures[var23] = faceColor[var23];
                            faceColor[var23] = 127;
                            if(faceTextures[var23] != -1) {
                                var9 = true;
                            }
                        } else {
                            texturePos[var23] = -1;
                            faceTextures[var23] = -1;
                        }
                    }

                    if(var15 == 255) {
                        facePriorities[var23] = var7.getSByte();
                    }

                    if(var16 == 1) {
                        faceAlphas[var23] = var8.getSByte();
                    }

                    if(var25 == 1) {
                        skinValues[var23] = var38.getSByte() & 0xFF;
                    }
                }

                var5.position(var39);
                var6.position(var24);

                var23 = 0;
                var27 = 0;
                var4 = 0;
                var33 = 0;

                int var31;
                int var44;
                for(var3 = 0; var3 < var11; var3++) {
                    var31 = var6.getSByte() & 0xFF;
                    if(var31 == 1) {
                        var23 = var5.getSmartA() + var33;
                        var27 = var5.getSmartA() + var23;
                        var4 = var5.getSmartA() + var27;
                        var33 = var4;
                        triangleX[var3] = var23;
                        triangleY[var3] = var27;
                        triangleZ[var3] = var4;
                    }

                    if(var31 == 2) {
                        var27 = var4;
                        var4 = var5.getSmartA() + var33;
                        var33 = var4;
                        triangleX[var3] = var23;
                        triangleY[var3] = var27;
                        triangleZ[var3] = var4;
                    }

                    if(var31 == 3) {
                        var23 = var4;
                        var4 = var5.getSmartA() + var33;
                        var33 = var4;
                        triangleX[var3] = var23;
                        triangleY[var3] = var27;
                        triangleZ[var3] = var4;
                    }

                    if(var31 == 4) {
                        var44 = var23;
                        var23 = var27;
                        var27 = var44;
                        var4 = var5.getSmartA() + var33;
                        var33 = var4;
                        triangleX[var3] = var23;
                        triangleY[var3] = var44;
                        triangleZ[var3] = var4;
                    }
                }

                var5.position(var35);

                for(var3 = 0; var3 < var12; var3++) {
                    textureRenderTypes[var3] = 0;
                    texTriX[var3] = (short)(var5.getShort() & 0xFFFF);
                    texTriY[var3] = (short)(var5.getShort() & 0xFFFF);
                    texTriZ[var3] = (short)(var5.getShort() & 0xFFFF);
                }

                if(texturePos != null) {
                    bool var45 = false;

                    for(var31 = 0; var31 < var11; var31++) {
                        var44 = texturePos[var31] & 0xff;
                        if(var44 != 255) {
                            if((texTriX[var44] & 0xffff) == triangleX[var31] && (texTriY[var44] & 0xffff) == triangleY[var31] && (texTriZ[var44] & 0xffff) == triangleZ[var31]) {
                                texturePos[var31] = -1;
                            } else {
                                var45 = true;
                            }
                        }
                    }

                    if(!var45) {
                        texturePos = null;
                    }
                }

                if(!var9) {
                    faceTextures = null;
                }

                if(!var2) {
                    faceType = null;
                }
            }
        }

    }
}
