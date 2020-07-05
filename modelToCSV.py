import numpy as np
import csv
#引用
#http://www.cloud.teu.ac.jp/public/MDF/toudouhk/blog/2015/01/15/OBJTips/
def loadOBJ(fliePath):
    numVertices = 0
    numUVs = 0
    numNormals = 0
    numFaces = 0
    vertices = []
    uvs = []
    normals = []
    vertexColors = []
    faceVertIDs = []
    uvIDs = []
    normalIDs = []
    for line in open(fliePath, "r"):
        vals = line.split()
        if len(vals) == 0:
            continue
        if vals[0] == "v":
            v = map(float, vals[1:4])
            vertices.append(v)
            if len(vals) == 7:
                vc = map(float, vals[4:7])
                vertexColors.append(vc)
            numVertices += 1
        if vals[0] == "vt":
            vt = map(float, vals[1:3])
            uvs.append(vt)
            numUVs += 1
        if vals[0] == "vn":
            vn = map(float, vals[1:4])
            normals.append(vn)
            numNormals += 1
        if vals[0] == "f":
            fvID = []
            uvID = []
            nvID = []
            for f in vals[1:]:
                w = f.split("/")
                if numVertices > 0:
                    fvID.append(int(w[0])-1)
                if numUVs > 0:
                    uvID.append(int(w[1])-1)
                if numNormals > 0:
                    nvID.append(int(w[2])-1)
            faceVertIDs.append(fvID)
            uvIDs.append(uvID)
            normalIDs.append(nvID)
            numFaces += 1
    print("numVertices: {0}" .format(numVertices))
    print("numUVs: {0}".format(numUVs))
    print("numNormals: {0}".format(numNormals))
    print("numFaces: {0}".format(numFaces))
    return vertices, uvs, normals, faceVertIDs, uvIDs, normalIDs, vertexColors
#ここまで

def writelistToCSV(filename,contents,addamount):
    with open('./csv/{0}'.format(filename),'w') as f:
        writer = csv.writer(f)
        writer.writerows(contents)
        for i in range(0,addamount):
            writer.writerow([0,0,0])
limit = 100
minpos,maxpos = 0,0

def normalize(innum):
    return ( (innum - minpos)/(maxpos - minpos) ) * (limit)

path1 = "./model/bodhisattva-avalokitesvara/source/Bodhisattva_-_CleanUp_-_LowPoly.obj"
path2 = "./model/pan-poursuivant-syrinx-cap-re/source/syrinx_-_CleanUp_-_LowPoly.obj"
vertices1, uvs1, normals1, faceVertIDs1, uvIDs1, normalIDs1, vertexColors1 = loadOBJ(path1)
vertices2, uvs2, normals2, faceVertIDs2, uvIDs2, normalIDs2, vertexColors2 = loadOBJ(path2)


vertexcount = max(len(vertices1),len(vertices2))

writelistToCSV("vertices1.csv",list(vertices1),abs(vertexcount - len(vertices1)))
writelistToCSV("uvs1.csv",list(uvs1),abs(vertexcount - len(vertices1)))
writelistToCSV("vcolors1.csv",list(vertexColors1),abs(vertexcount - len(vertices1)))

writelistToCSV("vertices2.csv",list(vertices2),abs(vertexcount - len(vertices2)))
writelistToCSV("uvs2.csv",list(uvs2),abs(vertexcount - len(vertices2)))
writelistToCSV("vcolors2.csv",list(vertexColors2),abs(vertexcount - len(vertices2)))
