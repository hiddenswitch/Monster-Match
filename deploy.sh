#!/usr/bin/env bash
set -e
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
CLOUDFRONT_DISTRIBUTION_ID=E2VE6A94JJLS9

OPTIND=1

usage="$(basename "$0") [-hWG] -- build and upload the Monster Match client

where:
    -h  show this help text
    -W  build for WebGL
    -G  upload the WebGL build to monstermatch
"
build_webgl=false
upload_webgl=false
while getopts "hWG" opt; do
  case "$opt" in
  h) echo "$usage"
     exit
     ;;
  W) build_webgl=true
     echo "Building WebGL"
     ;;
  G) upload_webgl=true
     echo "Uploading WebGL build to monstermatch"
  esac
done
shift $((OPTIND-1))
[ "${1:-}" = "--" ] && shift

BUILD_PATH=${DIR}/obj/webgl
if [ "$build_webgl" = true ] ; then
  BACKGROUND_COLOR="#000000"
  UNITY_VERSION=$(cat ${DIR}/ProjectSettings/ProjectVersion.txt | tr " " "\n" | sed -n 2p)
  UNITY=/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity
  BUILD_COMMAND=WebGL
  mkdir -pv ${DIR}/obj
  echo "Building WebGL Player to ${BUILD_PATH}"
  brotli_script_path=/Applications/Unity/Hub/Editor/${UNITY_VERSION}/PlaybackEngines/WebGLSupport/BuildTools/Brotli/python
  
  # Fix issue with brotli compression script not working
  if [[ ! -x "${brotli_script_path}/bro.py" ]] ; then
    echo "Fixing brotli script executability. Need sudo."
    sudo chmod -R "=rw,+X" ${brotli_script_path}
    sudo chmod "+x" ${brotli_script_path}/bro.py
  fi
  build_failed=false
  
  "${UNITY}" \
    -buildTarget WebGL \
    -batchmode \
    -nographics \
    -silent-crashes \
    -logFile "${DIR}"/unity.log \
    -projectPath "${DIR}" \
    -buildPath "${BUILD_PATH}" \
    -executeMethod MonsterMatch.Editor.BuildScripts.Build${BUILD_COMMAND} \
    -quit > /dev/null || build_failed=true
  
  if [ "$build_failed" = true ] ; then
    echo "Build failed"
    tail unity.log
    exit 1
  fi
  
  # Compress the bundle files with brotli
  brotli_pythonpath=$(ls /Applications/Unity/Hub/Editor/${UNITY_VERSION}/PlaybackEngines/WebGLSupport/BuildTools/Brotli/dist/*macosx*)
  export PYTHONPATH=${brotli_pythonpath}
  find "${BUILD_PATH}/StreamingAssets" -name "*.bundle" -exec python "${brotli_script_path}/bro.py" --input {} --output {} --force \;
fi

if [ "$upload_webgl" = true ] ; then 
  echo "Deploying to AWS"
  
    # Patch the JSON file to have the right color
  for json_file in "${BUILD_PATH}"/Build/*.json ; do
    cp ${json_file} ${json_file}.bak 
    jq '.backgroundColor="#000000"' ${json_file} > ${json_file}.tmp
    cp ${json_file}.tmp ${json_file}
    rm ${json_file}.tmp 
  done
  
  cd ${BUILD_PATH}
  if [[ -e ".DS_STORE" ]] ; then
    rm ./.DS_Store
  fi
  aws s3 sync . s3://monstermatch --acl=public-read --delete --exclude ".DS_Store" --exclude "*.bak"  --exclude "*.unityweb" --exclude "*.bundle"
  
  echo "Setting files to brotli content encoding type"
  aws s3 cp --recursive . s3://monstermatch --exclude "*" --include "*.unityweb" --include "*.bundle" --acl=public-read --content-encoding=br
  
  echo "Preventing addressables from being cached in the browser (deals with changes)"
  aws s3 cp --recursive . s3://monstermatch --exclude "*" --include "*.json" --include "*.xml" --cache-control="no-cache, no-store, must-revalidate" --expires=0
  
  echo "Creating CloudFront invalidation"
  aws cloudfront create-invalidation --distribution-id ${CLOUDFRONT_DISTRIBUTION_ID} --paths "/index.html" "/TemplateData/*" "/StreamingAssets/*"
fi