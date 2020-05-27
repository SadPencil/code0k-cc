#!/bin/bash

extens=(".cs" ".c" ".h" ".java" ".sln" ".csproj")
filesCount=0
linesCount=0
function funCount()
{
    for file in ` ls $1 `
    do
        if [ -d $1"/"$file ];then
            funCount $1"/"$file
        else
            fileName=$1"/"$file
            
            EXTENSION="."${fileName##*.}

            if [[ "${extens[@]/$EXTENSION/}" != "${extens[@]}" ]];then
                echo "=====文件名======"
                echo $fileName
	            echo "=====文件内容======"
            	cat $fileName
	            echo "==============="
	            echo ""
            fi
        fi
    done
}

if [ $# -gt 0 ];then
    for m_dir in $@
    do
        funCount $m_dir
    done
else
    funCount "."
fi
