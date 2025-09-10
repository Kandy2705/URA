def PROJECT_NAME = "URA"
def CUSTOME_WORKSPACE = "C:\\Jenkins\\Unity_Projects\\${PROJECT_NAME}"
def UNITY_VERSION = "6000.0.44f1"
def UNITY_INSTALLATION = "C:\\Program Files\\Unity\\Hub\\Editor\\${UNITY_VERSION}\\Editor"
pipeline{
    environment{
        PROJECT_PATH = "${CUSTOME_WORKSPACE}\\${PROJECT_NAME}"
    }

    agent{
        label{
            label ""
            customWorkspace "${CUSTOME_WORKSPACE}"
        }
    }

    options { skipDefaultCheckout(true) }

    stages{
        stage('Checkout') {
            steps {
                deleteDir() // xóa sạch workspace
                checkout([$class: 'GitSCM',
                branches: [[name: '*/main']],
                userRemoteConfigs: [[url: 'https://github.com/Kandy2705/URA']],
                extensions: [[$class: 'CleanBeforeCheckout'], [$class: 'PruneStaleBranch']]
                ])
            }
        }

        stage('Build Windows'){
            when{expression{BUILD_WINDOWS == 'true'}}
            steps{
                script{
                    withEnv(["UNITY_PATH=${UNITY_INSTALLATION}"]){
                        bat '''
                        "%UNITY_PATH%/Unity.exe" -quit -batchmode -projectPath %PROJECT_PATH% -executeMethod BuildScript.BuildWindows -logFile -
                        '''
                    }
                }
            }
        }

        stage('Deploy Windows'){
            when{expression{DEPLOY_WINDOWS == 'true'}}
            steps{
                echo 'Deploy Windows'
            }
        }
    }
}