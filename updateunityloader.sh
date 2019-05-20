#!/usr/bin/env bash
set -e
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
PROJECT_VERSION=$(cat ProjectSettings/ProjectVersion.txt | tr " " "\n" | sed -n 2p)
UNITY_LOADERS_DIR=/Applications/Unity/Hub/Editor/${PROJECT_VERSION}/PlaybackEngines/WebGLSupport/BuildTools
cp "${DIR}"/Assets/WebGLTemplates/UnityLoader.js "${UNITY_LOADERS_DIR}"/UnityLoader.js && \
cp "${DIR}"/Assets/WebGLTemplates/UnityLoader.js "${UNITY_LOADERS_DIR}"/UnityLoader.min.js && \
echo "Updated your UnityLoader.js and UnityLoader.min.js" || echo "Failed to update. Make sure you run as sudo" 
