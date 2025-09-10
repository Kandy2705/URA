pipeline {
  agent any
  stages {
    stage('Checkout') {
      steps {
        checkout scm
      }
    }
    stage('Build') {
      steps {
        sh '/Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -executeMethod MyEditorScript.PerformBuild -projectPath .'
      }
    }
  }
}
