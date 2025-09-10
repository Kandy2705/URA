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
                git branch: 'main',
                    url: 'git@github.com:Kandy2705/URA.git',   // dùng SSH URL
                    credentialsId: 'SHA256:HvLZ5nlO9kJIw7HdP7M7Cn2xy4Fmkj0ghhhxiKdBw6M'                // ID của SSH private key trong Jenkins
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
    
    post{
        always{
            archiveArtifacts artifacts: "${PROJECT_NAME}//Builds/**", allowEmptyArchive: true
        }
        failure{
            echo 'Build failed!'
        }
    }
}