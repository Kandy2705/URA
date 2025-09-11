def PROJECT_NAME = "URA"
def CUSTOME_WORKSPACE = "C:\\Jenkins\\Unity_Projects\\${PROJECT_NAME}"
def UNITY_VERSION = "6000.0.44f1"
def UNITY_INSTALLATION = "C:\\Program Files\\Unity\\Hub\\Editor\\${UNITY_VERSION}\\Editor"
pipeline{
      parameters {
        booleanParam(name: 'BUILD_WINDOWS',  defaultValue: true,  description: 'Build Windows')
        booleanParam(name: 'DEPLOY_WINDOWS', defaultValue: false, description: 'Deploy Windows')
    }
    environment{
        PROJECT_PATH = "${CUSTOME_WORKSPACE}"
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
                deleteDir()
                git branch: 'main',
                    url: 'git@github.com:Kandy2705/URA.git',
                    credentialsId: 'github-ssh-ura'   // <-- ID credential trong Jenkins, KHÔNG phải public key
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