//Maya ASCII 2023 scene
//Name: rabbitModel.015.ma
//Last modified: Sun, Jun 01, 2025 09:30:47 AM
//Codeset: 1252
requires maya "2023";
requires -nodeType "displayPoints" "Type" "2.0a";
requires "stereoCamera" "10.0";
currentUnit -l centimeter -a degree -t film;
fileInfo "application" "maya";
fileInfo "product" "Maya 2023";
fileInfo "version" "2023";
fileInfo "cutIdentifier" "202202161415-df43006fd3";
fileInfo "osv" "Windows 11 Pro v2009 (Build: 26100)";
fileInfo "UUID" "23EC7142-4B6E-F929-2EFB-0E93544E0E8D";
createNode transform -s -n "persp";
	rename -uid "445DD740-4CDA-5B18-1DFE-0C81A3218031";
	setAttr ".v" no;
	setAttr ".t" -type "double3" 2.8267421356446385 5.380281380477391 11.893742335019706 ;
	setAttr ".r" -type "double3" -14.138352729667981 16.200000000000568 8.2801613946400353e-16 ;
	setAttr ".rp" -type "double3" 3.5527136788005009e-15 3.5527136788005009e-15 0 ;
	setAttr ".rpt" -type "double3" -2.3116966033727347e-16 -3.6332576967528385e-16 -2.0096514786423561e-15 ;
createNode camera -s -n "perspShape" -p "persp";
	rename -uid "9D352A22-4CA8-CD19-95F1-62885D98B28B";
	setAttr -k off ".v" no;
	setAttr ".fl" 34.999999999999993;
	setAttr ".coi" 13.711063111390303;
	setAttr ".imn" -type "string" "persp";
	setAttr ".den" -type "string" "persp_depth";
	setAttr ".man" -type "string" "persp_mask";
	setAttr ".tp" -type "double3" 2.7755575615628914e-16 0.55658499896526326 -0.00083161622751498632 ;
	setAttr ".hc" -type "string" "viewSet -p %camera";
createNode transform -s -n "top";
	rename -uid "87D3FC90-43AC-3BC7-EFFD-F9A4BA73935E";
	setAttr ".v" no;
	setAttr ".t" -type "double3" 0 1000.1 0 ;
	setAttr ".r" -type "double3" -90 0 0 ;
createNode camera -s -n "topShape" -p "top";
	rename -uid "8E3506E9-45DB-72C4-E430-D391DFA193FF";
	setAttr -k off ".v" no;
	setAttr ".rnd" no;
	setAttr ".coi" 1000.1;
	setAttr ".ow" 6.6558954648004445;
	setAttr ".imn" -type "string" "top";
	setAttr ".den" -type "string" "top_depth";
	setAttr ".man" -type "string" "top_mask";
	setAttr ".hc" -type "string" "viewSet -t %camera";
	setAttr ".o" yes;
createNode transform -s -n "front";
	rename -uid "EE94800D-4C07-A39B-0307-C29FC189C3D9";
	setAttr ".t" -type "double3" -0.35682431800675485 1.5668252207090363 1000.1095776136274 ;
createNode camera -s -n "frontShape" -p "front";
	rename -uid "96B66DEC-43E6-5A08-AB9C-8FA8EAE723BE";
	setAttr -k off ".v";
	setAttr ".rnd" no;
	setAttr ".coi" 1000.1095776136274;
	setAttr ".ow" 5.6399151680105648;
	setAttr ".imn" -type "string" "front";
	setAttr ".den" -type "string" "front_depth";
	setAttr ".man" -type "string" "front_mask";
	setAttr ".tp" -type "double3" 0.3370801064056751 1.9143606043039307 0 ;
	setAttr ".hc" -type "string" "viewSet -f %camera";
	setAttr ".o" yes;
createNode transform -s -n "side";
	rename -uid "FD24BE21-47AA-794F-7BF2-20AF545272B9";
	setAttr ".v" no;
	setAttr ".t" -type "double3" 1000.1004478954167 0.56117866296286356 0.30673387058396911 ;
	setAttr ".r" -type "double3" 0 90 0 ;
createNode camera -s -n "sideShape" -p "side";
	rename -uid "D447139A-43A7-BB3D-FE33-F789E056E8E8";
	setAttr -k off ".v" no;
	setAttr ".rnd" no;
	setAttr ".coi" 999.34639682426553;
	setAttr ".ow" 1.6943016491515055;
	setAttr ".imn" -type "string" "side";
	setAttr ".den" -type "string" "side_depth";
	setAttr ".man" -type "string" "side_mask";
	setAttr ".tp" -type "double3" 0.7540510711511943 1.9703369652819758 0.001457342018856056 ;
	setAttr ".hc" -type "string" "viewSet -s %camera";
	setAttr ".o" yes;
createNode transform -n "transform1";
	rename -uid "2CA9BEDF-4B9E-83FD-38E6-07A37B6BEF65";
	setAttr ".hio" yes;
createNode displayPoints -n "displayPoints1" -p "transform1";
	rename -uid "684EDE40-4053-4624-C815-C08BDE809533";
	setAttr -k off ".v";
	setAttr ".hio" yes;
createNode transform -n "root_CTRL";
	rename -uid "472CA664-4F93-4433-B046-DBA167282245";
createNode nurbsCurve -n "root_CTRLShape" -p "root_CTRL";
	rename -uid "A471E2D5-423A-71FF-B089-D3814BE11718";
	setAttr -k off ".v";
	setAttr ".tw" yes;
createNode transform -n "CTRL_GRP" -p "root_CTRL";
	rename -uid "66D0C75F-4B40-173D-DD49-C4AA55D8E5D9";
createNode transform -n "hip_CTRL" -p "CTRL_GRP";
	rename -uid "B52E6854-4602-35A0-AF10-83A2C3031F70";
	setAttr ".rp" -type "double3" 1.2554342668302078e-17 0.80927163362503052 -0.00083161622751504183 ;
	setAttr ".sp" -type "double3" 1.2554342668302078e-17 0.80927163362503052 -0.00083161622751504183 ;
createNode nurbsCurve -n "hip_CTRLShape" -p "hip_CTRL";
	rename -uid "454329A1-4B0F-DAF8-45E9-01B6DA610A9F";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 10 1 no 3
		15 0 0 0 1 1 1 2 2 2 3 3 3 4 4 4
		13
		-0.77932312586013452 0.80927163362503052 0.70073006041367603
		-0.77932312586013452 0.80927163362503052 0.23302227598621536
		-0.77932312586013452 0.80927163362503052 -0.23468550844124544
		-0.77932312586013452 0.80927163362503052 -0.70239329286870611
		-0.25977437528671149 0.80927163362503052 -0.70239329286870611
		0.25977437528671171 0.80927163362503052 -0.70239329286870611
		0.77932312586013452 0.80927163362503052 -0.70239329286870611
		0.77932312586013508 0.80927163362503052 -0.23468550844124544
		0.77932312586013508 0.80927163362503052 0.23302227598621536
		0.77932312586013508 0.80927163362503052 0.70073006041367603
		0.25977437528671182 0.80927163362503052 0.70073006041367614
		-0.25977437528671132 0.80927163362503052 0.70073006041367614
		-0.77932312586013452 0.80927163362503052 0.70073006041367614
		;
createNode transform -n "chest_CTRL" -p "hip_CTRL";
	rename -uid "F7AD5B14-469E-66CF-6A22-63BFF9636C15";
	setAttr ".rp" -type "double3" 1.2554342668302079e-17 1.6884008646011353 -0.00083161622751504204 ;
	setAttr ".sp" -type "double3" 1.2554342668302079e-17 1.6884008646011353 -0.00083161622751504204 ;
createNode nurbsCurve -n "chest_CTRLShape" -p "chest_CTRL";
	rename -uid "9BDE433A-40CB-A0FE-463B-BAAE23E806A7";
	setAttr -k off ".v";
	setAttr ".tw" yes;
	setAttr -s 11 ".cp[1:10]" -type "double3" 0 -0.20355845394889727 0 
		0 0 0 0 -0.20355845394889727 0 0 0 0 0 -0.20355845394889749 0 0 0 0 0 -0.20355845394889727 
		0 0 0 0 0 0 0 0 0 0;
createNode transform -n "head_CTRL" -p "chest_CTRL";
	rename -uid "DB0F4957-477A-5286-CAAF-FF8F1D2E4C1F";
	setAttr ".rp" -type "double3" 1.2554342668302079e-17 2.2244033813476567 -0.00083161622751504204 ;
	setAttr ".sp" -type "double3" 1.2554342668302079e-17 2.2244033813476567 -0.00083161622751504204 ;
createNode nurbsCurve -n "head_CTRLShape" -p "head_CTRL";
	rename -uid "F2A97FF4-406B-13DB-8F98-AC8E4465064C";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		0.90561260234007035 3.5421127548738056 -0.9064442185675855
		7.6467478402599288e-17 3.5421127548738056 -1.2815612407128347
		-0.90561260234007035 3.5421127548738056 -0.90644421856758528
		-1.2807296244853201 3.5421127548738056 -0.00083161622751516835
		-0.90561260234007035 3.5421127548738056 0.90478098611255531
		-1.3024615577314978e-16 3.5421127548738056 1.2798980082578053
		0.90561260234007035 3.5421127548738056 0.9047809861125552
		1.2807296244853201 3.5421127548738056 -0.00083161622751492734
		0.90561260234007035 3.5421127548738056 -0.9064442185675855
		7.6467478402599288e-17 3.5421127548738056 -1.2815612407128347
		-0.90561260234007035 3.5421127548738056 -0.90644421856758528
		;
createNode transform -n "ear1_L_CTRL_0" -p "head_CTRL";
	rename -uid "A95B71FE-4C47-8FDF-712C-929C6967D965";
	setAttr ".rp" -type "double3" 0.43539941621753558 3.3431277275085458 -0.082778676124220657 ;
	setAttr ".sp" -type "double3" 0.43539941621753558 3.3431277275085458 -0.082778676124220657 ;
createNode transform -n "ear1_L_CTRL_SDK" -p "ear1_L_CTRL_0";
	rename -uid "5CDD068D-4754-C7DE-9699-0390F2D9C542";
	setAttr ".rp" -type "double3" 0.43539941621753558 3.3431277275085458 -0.082778676124220657 ;
	setAttr ".sp" -type "double3" 0.43539941621753558 3.3431277275085458 -0.082778676124220657 ;
createNode transform -n "ear1_L_CTRL" -p "ear1_L_CTRL_SDK";
	rename -uid "994CB397-46B0-5C2A-9B06-78A2F88A10A1";
	setAttr ".rp" -type "double3" 0.43539941621753558 3.3431277275085458 -0.082778676124220657 ;
	setAttr ".sp" -type "double3" 0.43539941621753558 3.3431277275085458 -0.082778676124220657 ;
createNode nurbsCurve -n "ear1_L_CTRLShape" -p "ear1_L_CTRL";
	rename -uid "014BF25A-40C7-4411-BDD4-FCA1FC43F702";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		0.95892062102488973 3.5006325312036877 -0.50587662209806095
		0.56216601099066743 3.6804998032921987 -0.6726976535171546
		0.16616361115020989 3.8381821755169336 -0.48318754432537681
		0.0028862565977644805 3.881311452775805 -0.048358746280464326
		0.16797960720175359 3.7846230893859154 0.3770719280332645
		0.56473421723597628 3.6047558172974044 0.54389295945235916
		0.96073661707643354 3.4470734450726699 0.35438285026058036
		1.1240139716288797 3.4039441678137976 -0.080445947784332159
		0.95892062102488973 3.5006325312036877 -0.50587662209806095
		0.56216601099066743 3.6804998032921987 -0.6726976535171546
		0.16616361115020989 3.8381821755169336 -0.48318754432537681
		;
createNode transform -n "ear2_L_CTRL_0" -p "ear1_L_CTRL";
	rename -uid "920802DA-4206-2928-4C2C-949526F08F42";
	setAttr ".rp" -type "double3" 0.83484168447144214 4.2857763639357298 -0.15021437162334836 ;
	setAttr ".sp" -type "double3" 0.83484168447144214 4.2857763639357298 -0.15021437162334836 ;
createNode transform -n "ear2_L_CTRL_SDK" -p "ear2_L_CTRL_0";
	rename -uid "6337A4EA-4EC2-2586-57A6-CF84B61417C5";
	setAttr ".rp" -type "double3" 0.83484168447144214 4.2857763639357298 -0.15021437162334836 ;
	setAttr ".sp" -type "double3" 0.83484168447144214 4.2857763639357298 -0.15021437162334836 ;
createNode transform -n "ear2_L_CTRL" -p "ear2_L_CTRL_SDK";
	rename -uid "FEE1E370-4620-91DA-0796-948E9B201572";
	setAttr ".rp" -type "double3" 0.83484168447144214 4.2857763639357298 -0.15021437162334836 ;
	setAttr ".sp" -type "double3" 0.83484168447144214 4.2857763639357298 -0.15021437162334836 ;
createNode nurbsCurve -n "ear2_L_CTRLShape" -p "ear2_L_CTRL";
	rename -uid "4EED8451-45F4-0734-BB6E-18AE7342B7F4";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		1.2430812223406875 4.1555329025773693 -0.58361515284034926
		0.85056486490038619 4.3382550755447351 -0.75722168878753704
		0.44883808160844857 4.4902359309874127 -0.57525157079965006
		0.27322697374877414 4.5224471450081456 -0.14430042604735988
		0.42660214660219686 4.4160198252940903 0.2831864095936526
		0.81911850404249831 4.2332976523267245 0.45679294554084127
		1.2208452873344358 4.0813167968840469 0.2748228275529534
		1.39645639519411 4.049105582863314 -0.15612831719933667
		1.2430812223406875 4.1555329025773693 -0.58361515284034926
		0.85056486490038619 4.3382550755447351 -0.75722168878753704
		0.44883808160844857 4.4902359309874127 -0.57525157079965006
		;
createNode transform -n "ear1_R_CTRL_0" -p "head_CTRL";
	rename -uid "6559BB42-481D-A478-E369-FE919227A324";
	setAttr ".rp" -type "double3" -0.43539941621753553 3.3431277275085458 -0.082778676124220643 ;
	setAttr ".sp" -type "double3" -0.43539941621753553 3.3431277275085458 -0.082778676124220643 ;
createNode transform -n "ear1_R_CTRL_SDK" -p "ear1_R_CTRL_0";
	rename -uid "6A2F2402-40FD-2EAB-AAE3-10992496397E";
	setAttr ".rp" -type "double3" -0.43539941621753553 3.3431277275085458 -0.082778676124220643 ;
	setAttr ".sp" -type "double3" -0.43539941621753553 3.3431277275085458 -0.082778676124220643 ;
createNode transform -n "ear1_R_CTRL" -p "ear1_R_CTRL_SDK";
	rename -uid "D584A5DE-46BD-52C9-EE54-D495856E344D";
	setAttr ".rp" -type "double3" -0.43539941621753553 3.3431277275085458 -0.082778676124220643 ;
	setAttr ".sp" -type "double3" -0.43539941621753553 3.3431277275085458 -0.082778676124220643 ;
createNode nurbsCurve -n "ear1_R_CTRLShape" -p "ear1_R_CTRL";
	rename -uid "55586B91-4961-31AB-B4B0-998BCF1F1CDE";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		-0.9589206210248894 3.5006325312036872 -0.50587662209806095
		-0.56216601099066732 3.6804998032921987 -0.67269765351715438
		-0.16616361115020989 3.8381821755169336 -0.48318754432537653
		-0.0028862565977648136 3.881311452775805 -0.048358746280464202
		-0.16797960720175381 3.7846230893859154 0.37707192803326445
		-0.56473421723597628 3.6047558172974044 0.54389295945235894
		-0.96073661707643332 3.4470734450726694 0.35438285026058008
		-1.1240139716288793 3.4039441678137976 -0.08044594778433227
		-0.9589206210248894 3.5006325312036872 -0.50587662209806095
		-0.56216601099066732 3.6804998032921987 -0.67269765351715438
		-0.16616361115020989 3.8381821755169336 -0.48318754432537653
		;
createNode transform -n "ear2_R_CTRL_0" -p "ear1_R_CTRL";
	rename -uid "57AF91BB-43E0-8907-E1BA-DBA81B390548";
	setAttr ".rp" -type "double3" -0.83484168447144214 4.2857763639357298 -0.15021437162334833 ;
	setAttr ".sp" -type "double3" -0.83484168447144214 4.2857763639357298 -0.15021437162334833 ;
createNode transform -n "ear2_R_CTRL_SDK" -p "ear2_R_CTRL_0";
	rename -uid "2B448933-4A70-2C9A-A96C-0EB0C2722148";
	setAttr ".rp" -type "double3" -0.83484168447144214 4.2857763639357298 -0.15021437162334833 ;
	setAttr ".sp" -type "double3" -0.83484168447144214 4.2857763639357298 -0.15021437162334833 ;
createNode transform -n "ear2_R_CTRL" -p "ear2_R_CTRL_SDK";
	rename -uid "F64D1D71-4FE1-5B6B-06D0-D48E5D892F61";
	setAttr ".rp" -type "double3" -0.83484168447144214 4.2857763639357298 -0.15021437162334833 ;
	setAttr ".sp" -type "double3" -0.83484168447144214 4.2857763639357298 -0.15021437162334833 ;
createNode nurbsCurve -n "ear2_R_CTRLShape" -p "ear2_R_CTRL";
	rename -uid "0AC4D1C9-4478-5B5B-BEA3-C785391A9635";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		-1.2430812223406873 4.1555329025773693 -0.58361515284034926
		-0.85056486490038608 4.3382550755447351 -0.75722168878753671
		-0.44883808160844874 4.4902359309874127 -0.57525157079964973
		-0.27322697374877436 4.5224471450081456 -0.14430042604735974
		-0.42660214660219714 4.4160198252940912 0.28318640959365254
		-0.81911850404249842 4.2332976523267245 0.4567929455408411
		-1.2208452873344355 4.0813167968840469 0.27482282755295312
		-1.39645639519411 4.049105582863314 -0.15612831719933676
		-1.2430812223406873 4.1555329025773693 -0.58361515284034926
		-0.85056486490038608 4.3382550755447351 -0.75722168878753671
		-0.44883808160844874 4.4902359309874127 -0.57525157079964973
		;
createNode transform -n "shoulder_L_CTRL" -p "chest_CTRL";
	rename -uid "48E466A6-4CC1-5F36-423B-388412020A99";
	setAttr ".rp" -type "double3" 0.40845635533332825 1.8094612360000606 -0.00083161622751504172 ;
	setAttr ".sp" -type "double3" 0.40845635533332825 1.8094612360000606 -0.00083161622751504172 ;
createNode nurbsCurve -n "shoulder_L_CTRLShape" -p "shoulder_L_CTRL";
	rename -uid "0ED6B6F6-4598-7692-572C-5CB898B98F23";
	setAttr -k off ".v";
	setAttr ".tw" yes;
createNode transform -n "elbow_L_CTRL" -p "shoulder_L_CTRL";
	rename -uid "26219BFC-40F5-2481-656E-2498563752D5";
	setAttr ".rp" -type "double3" 0.8505588173866272 1.8094612360000608 -0.00083161622751504183 ;
	setAttr ".sp" -type "double3" 0.8505588173866272 1.8094612360000608 -0.00083161622751504183 ;
createNode nurbsCurve -n "elbow_L_CTRLShape" -p "elbow_L_CTRL";
	rename -uid "6606BF9B-4BB0-5C8F-34D2-FB9937517B65";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		0.85055881738662709 1.6182832456497864 -0.19200960657778945
		0.8505588173866272 1.8094612360000608 -0.2711981230081057
		0.85055881738662731 2.0006392263503354 -0.1920096065777894
		0.85055881738662731 2.0798277427806515 -0.00083161622751505581
		0.85055881738662731 2.0006392263503354 0.19034637412275934
		0.8505588173866272 1.8094612360000608 0.26953489055307578
		0.85055881738662709 1.6182832456497864 0.19034637412275932
		0.85055881738662709 1.5390947292194701 -0.00083161622751500497
		0.85055881738662709 1.6182832456497864 -0.19200960657778945
		0.8505588173866272 1.8094612360000608 -0.2711981230081057
		0.85055881738662731 2.0006392263503354 -0.1920096065777894
		;
createNode transform -n "shoulder_R_CTRL" -p "chest_CTRL";
	rename -uid "96D6A1CC-40B5-2F61-4B46-749F17A1E01E";
	setAttr ".rp" -type "double3" -0.40845635533332825 1.8094612360000604 -0.00083161622751504172 ;
	setAttr ".sp" -type "double3" -0.40845635533332825 1.8094612360000604 -0.00083161622751504172 ;
createNode nurbsCurve -n "shoulder_R_CTRLShape" -p "shoulder_R_CTRL";
	rename -uid "D4EDD60F-4A16-F6F3-B21B-10A20F3B3E76";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		-0.40845635533332819 1.618283245649786 -0.19200960657778945
		-0.4084563553333283 1.8094612360000604 -0.2711981230081057
		-0.40845635533332836 2.0006392263503345 -0.1920096065777894
		-0.40845635533332836 2.0798277427806511 -0.00083161622751505571
		-0.4084563553333283 2.0006392263503345 0.19034637412275934
		-0.40845635533332819 1.8094612360000604 0.26953489055307578
		-0.40845635533332814 1.618283245649786 0.19034637412275932
		-0.40845635533332814 1.5390947292194697 -0.00083161622751500486
		-0.40845635533332819 1.618283245649786 -0.19200960657778945
		-0.4084563553333283 1.8094612360000604 -0.2711981230081057
		-0.40845635533332836 2.0006392263503345 -0.1920096065777894
		;
createNode transform -n "elbow_R_CTRL" -p "shoulder_R_CTRL";
	rename -uid "BD8D98F9-4A4D-BBF4-D1A2-B99D01146773";
	setAttr ".rp" -type "double3" -0.8505588173866272 1.8094612360000606 -0.00083161622751498773 ;
	setAttr ".sp" -type "double3" -0.8505588173866272 1.8094612360000606 -0.00083161622751498773 ;
createNode nurbsCurve -n "elbow_R_CTRLShape" -p "elbow_R_CTRL";
	rename -uid "1F6E59B6-4AF6-8601-59AD-B48795E17721";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 8 2 no 3
		13 -2 -1 0 1 2 3 4 5 6 7 8 9 10
		11
		-0.8505588173866272 1.6182832456497862 -0.1920096065777894
		-0.8505588173866272 1.8094612360000606 -0.27119812300810564
		-0.85055881738662731 2.000639226350335 -0.19200960657778934
		-0.85055881738662731 2.0798277427806515 -0.00083161622751500171
		-0.8505588173866272 2.000639226350335 0.1903463741227594
		-0.8505588173866272 1.8094612360000606 0.26953489055307583
		-0.85055881738662709 1.6182832456497862 0.19034637412275937
		-0.85055881738662709 1.5390947292194699 -0.00083161622751495086
		-0.8505588173866272 1.6182832456497862 -0.1920096065777894
		-0.8505588173866272 1.8094612360000606 -0.27119812300810564
		-0.85055881738662731 2.000639226350335 -0.19200960657778934
		;
createNode transform -n "leg_R_CTRL_0" -p "CTRL_GRP";
	rename -uid "CA69670C-40D1-FE0D-3DC7-4FB7B666F179";
createNode transform -n "leg_R_CTRL" -p "leg_R_CTRL_0";
	rename -uid "B67882B8-4A4A-DE33-6CD0-A1A69AF75D00";
	setAttr ".rp" -type "double3" -0.29430171847343456 0.54792958498001076 0.00066644005710259123 ;
	setAttr ".sp" -type "double3" -0.29430171847343456 0.54792958498001076 0.00066644005710259123 ;
createNode nurbsCurve -n "leg_R_CTRLShape" -p "leg_R_CTRL";
	rename -uid "5B335367-47F1-F8BB-4C4E-2FB7C0AE1386";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 10 1 no 3
		15 0 0 0 1 1 1 2 2 2 3 3 3 4 4 4
		13
		0.16251067867255492 0.54792958498001076 0.54417837939584968
		0.16251067867255498 0.54792958498001076 0.18183708650335173
		0.16251067867255498 0.54792958498001076 -0.18050420638914644
		0.16251067867255503 0.54792958498001076 -0.54284549928164449
		-0.14203091942477131 0.54792958498001076 -0.54284549928164449
		-0.44657251752209781 0.54792958498001076 -0.54284549928164449
		-0.75111411561942409 0.54792958498001076 -0.54284549928164449
		-0.75111411561942443 0.54792958498001076 -0.18050420638914655
		-0.75111411561942443 0.54792958498001076 0.18183708650335162
		-0.75111411561942454 0.54792958498001076 0.54417837939584968
		-0.44657251752209803 0.54792958498001076 0.5441783793958499
		-0.14203091942477156 0.54792958498001076 0.5441783793958499
		0.16251067867255492 0.54792958498001076 0.5441783793958499
		;
createNode transform -n "foot_R_CTRL" -p "leg_R_CTRL";
	rename -uid "DD5D028C-4CA4-97AF-0C64-34B4FCBDDA21";
	setAttr ".rp" -type "double3" -0.29430171847343456 0.30389836430549594 0.00066644005710259134 ;
	setAttr ".sp" -type "double3" -0.29430171847343456 0.30389836430549594 0.00066644005710259134 ;
createNode nurbsCurve -n "foot_R_CTRLShape" -p "foot_R_CTRL";
	rename -uid "7E5711B9-47FA-AE96-9991-6CA746E7996D";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 10 1 no 3
		15 0 0 0 1 1 1 2 2 2 3 3 3 4 4 4
		13
		0.0270239118415046 0.30389836430549594 0.36587150675007402
		0.027023911841504655 0.30389836430549594 0.12240146228809312
		0.027023911841504655 0.30389836430549594 -0.12106858217388786
		0.027023911841504711 0.30389836430549594 -0.36453862663586872
		-0.18719317503512145 0.30389836430549594 -0.36453862663586878
		-0.40141026191174767 0.30389836430549594 -0.36453862663586878
		-0.61562734878837366 0.30389836430549594 -0.36453862663586883
		-0.61562734878837388 0.30389836430549594 -0.12106858217388794
		-0.61562734878837388 0.30389836430549594 0.12240146228809304
		-0.615627348788374 0.30389836430549594 0.3658715067500739
		-0.40141026191174778 0.30389836430549594 0.36587150675007407
		-0.18719317503512162 0.30389836430549594 0.36587150675007407
		0.0270239118415046 0.30389836430549594 0.36587150675007413
		;
createNode pointConstraint -n "leg_R_CTRL_0_pointConstraint1" -p "leg_R_CTRL_0";
	rename -uid "71128CD3-424E-2CCF-D8AB-F59A29AD038E";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "hip_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".o" -type "double3" -1.2554342668302078e-17 -0.80927163362503052 0.00083161622751504183 ;
	setAttr -k on ".w0";
createNode transform -n "leg_L_CTRL_0" -p "CTRL_GRP";
	rename -uid "056E9DA1-42CD-831B-805C-798438F22409";
createNode transform -n "leg_L_CTRL" -p "leg_L_CTRL_0";
	rename -uid "3FE0E406-4B12-3041-B02D-71B53F506055";
	setAttr ".rp" -type "double3" 0.29430171847343456 0.54792958498001076 0.00066644005710259123 ;
	setAttr ".sp" -type "double3" 0.29430171847343456 0.54792958498001076 0.00066644005710259123 ;
createNode nurbsCurve -n "leg_L_CTRLShape" -p "leg_L_CTRL";
	rename -uid "88EFEC03-46C1-4022-F305-DB88F143F50C";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 10 1 no 3
		15 0 0 0 1 1 1 2 2 2 3 3 3 4 4 4
		13
		-0.16251067867255498 0.54792958498001076 0.54417837939584968
		-0.16251067867255498 0.54792958498001076 0.18183708650335167
		-0.16251067867255498 0.54792958498001076 -0.18050420638914649
		-0.16251067867255498 0.54792958498001076 -0.54284549928164449
		0.14203091942477139 0.54792958498001076 -0.54284549928164449
		0.44657251752209787 0.54792958498001076 -0.54284549928164449
		0.75111411561942409 0.54792958498001076 -0.54284549928164449
		0.75111411561942443 0.54792958498001076 -0.18050420638914649
		0.75111411561942443 0.54792958498001076 0.18183708650335167
		0.75111411561942443 0.54792958498001076 0.54417837939584968
		0.44657251752209792 0.54792958498001076 0.5441783793958499
		0.14203091942477147 0.54792958498001076 0.5441783793958499
		-0.16251067867255498 0.54792958498001076 0.5441783793958499
		;
createNode transform -n "foot_L_CTRL" -p "leg_L_CTRL";
	rename -uid "3A94F94E-4712-963B-AD09-82BD01DD7050";
	setAttr ".rp" -type "double3" 0.29430171847343456 0.30389836430549594 0.00066644005710259134 ;
	setAttr ".sp" -type "double3" 0.29430171847343456 0.30389836430549594 0.00066644005710259134 ;
createNode nurbsCurve -n "foot_L_CTRLShape" -p "foot_L_CTRL";
	rename -uid "0BA2814B-47B9-F8B9-9B39-7C80C97AD038";
	setAttr -k off ".v";
	setAttr ".cc" -type "nurbsCurve" 
		3 10 1 no 3
		15 0 0 0 1 1 1 2 2 2 3 3 3 4 4 4
		13
		-0.027023911841504655 0.30389836430549594 0.36587150675007396
		-0.027023911841504655 0.30389836430549594 0.12240146228809308
		-0.027023911841504655 0.30389836430549594 -0.1210685821738879
		-0.027023911841504655 0.30389836430549594 -0.36453862663586878
		0.18719317503512151 0.30389836430549594 -0.36453862663586878
		0.40141026191174772 0.30389836430549594 -0.36453862663586878
		0.61562734878837377 0.30389836430549594 -0.36453862663586878
		0.61562734878837388 0.30389836430549594 -0.1210685821738879
		0.61562734878837388 0.30389836430549594 0.12240146228809308
		0.61562734878837388 0.30389836430549594 0.36587150675007396
		0.40141026191174778 0.30389836430549594 0.36587150675007407
		0.18719317503512156 0.30389836430549594 0.36587150675007407
		-0.027023911841504655 0.30389836430549594 0.36587150675007407
		;
createNode pointConstraint -n "leg_L_CTRL_0_pointConstraint1" -p "leg_L_CTRL_0";
	rename -uid "89573B19-4725-CA60-C177-F48AA7C5D206";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "hip_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".o" -type "double3" -1.2554342668302078e-17 -0.80927163362503052 0.00083161622751504183 ;
	setAttr -k on ".w0";
createNode transform -n "JNT_GRP" -p "root_CTRL";
	rename -uid "783AE455-4E32-CDC7-09EE-CFAD9F920447";
createNode joint -n "pelvis_JNT" -p "JNT_GRP";
	rename -uid "5E75A536-416F-F1F4-B301-548B46407A5C";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 0 0 90 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 1 0 0 -1 0 0 0 0 0 1 0 1.2554342668302078e-17 0.80927163362503052 -0.0008316162105935776 1;
	setAttr ".liw" yes;
createNode joint -n "chest_JNT" -p "pelvis_JNT";
	rename -uid "90E998E8-4A18-6D87-93BC-41983E3AF1C2";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 1 0 0 -1 0 0 0 0 0 1 0 1.2554342668302078e-17 1.6884008890535362 -0.0008316162105935776 1;
	setAttr ".liw" yes;
createNode joint -n "head_JNT" -p "chest_JNT";
	rename -uid "11722EBB-4161-BC8C-A813-E8A3508C2118";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 1 0 0 -1 0 0 0 0 0 1 0 1.2554342668302078e-17 2.224403264145324 -0.0008316162105935776 1;
	setAttr ".liw" yes;
createNode joint -n "headTIP_JNT" -p "head_JNT";
	rename -uid "73DB2831-4525-DEFA-8B85-C592D401CBCD";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".t" -type "double3" 1.5496398486294836 -1.5407439555097887e-33 0 ;
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 0 0 -90 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 1.2554342668302079e-17 3.7740431127748075 -0.0008316162105935776 1;
	setAttr ".liw" yes;
createNode joint -n "ear1_L_JNT" -p "head_JNT";
	rename -uid "2D834979-4EEC-7C1E-582D-D684653013A8";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 1.510808227529902 -3.2290081270731563 -23.149038949592725 ;
	setAttr -av ".is" -type "double3" 1 1 1 ;
	setAttr -av ".is";
	setAttr ".bps" -type "matrix" 0.39250010579866973 0.91802556418698344 0.056326995723221626 0
		 -0.91974954629554095 0.39162205864115568 0.026323663780140404 0 0.0021069022710322322 -0.062138769579337685 0.99806529559843238 0
		 0.43539941621753542 3.3431277275085445 -0.082778676124220629 1;
	setAttr ".liw" yes;
createNode joint -n "ear2_L_JNT" -p "ear1_L_JNT";
	rename -uid "226359D1-4F30-7E72-BACB-AABA92226932";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" -0.93943469396697499 -1.8907033940993576 0.29716208945924844 ;
	setAttr -av ".is" -type "double3" 1 1 1 ;
	setAttr -av ".is";
	setAttr ".bps" -type "matrix" 0.38758303580364312 0.91749330548532126 0.089361203812677953 0
		 -0.92147374193802634 0.38831944049608025 0.0097033525929397686 0 -0.025797931621809423 -0.086104857716799738 0.99595201701769043 0
		 0.83484168447144191 4.2857763639357289 -0.15021437162334828 1;
	setAttr ".liw" yes;
createNode joint -n "earTIP_JNT" -p "ear2_L_JNT";
	rename -uid "5AB99C76-47AF-2556-BCAD-50BC70560A89";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".t" -type "double3" 0.7996859916651351 -0.00041214217747687076 0.088869983137121522 ;
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" -4.672089719616328 1.4783167287111059 -67.234776362604535 ;
	setAttr -av ".is" -type "double3" 1 1 1 ;
	setAttr -av ".is";
	setAttr ".bps" -type "matrix" 0.99999966392981798 -0.00081673337538256058 -7.1322119155933783e-05 0
		 0.00081706079500785261 0.99998853567882129 0.0047181482155655013 0 6.7467832378605153e-05 -0.0047182049044439678 0.9999888669333129 0
		 1.1428735252569491 5.0116707277058019 0.0097527710608611984 1;
	setAttr ".liw" yes;
createNode parentConstraint -n "ear2_L_JNT_parentConstraint1" -p "ear2_L_JNT";
	rename -uid "01C63910-48DC-798B-2DE6-79B4D0050096";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "ear2_L_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" 8.8817841970012523e-16 1.7763568394002505e-15 
		1.1102230246251565e-16 ;
	setAttr ".tg[0].tor" -type "double3" 0.5582031576092642 -5.1268586898377215 67.099009589418372 ;
	setAttr ".lr" -type "double3" 1.5530052155583569e-16 -8.724006798399073e-16 9.4826498461993301e-15 ;
	setAttr ".rst" -type "double3" 1.0183582287043063 1.2212453270876722e-15 -0.12503866795331808 ;
	setAttr ".rsrr" -type "double3" 1.5530052155583569e-16 -8.724006798399071e-16 9.4826498461993301e-15 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "ear1_L_JNT_parentConstraint1" -p "ear1_L_JNT";
	rename -uid "88CB4DD9-4335-330E-F9D9-0A92F42CB276";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "ear1_L_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" 2.2204460492503131e-16 -1.3322676295501878e-15 
		4.163336342344337e-17 ;
	setAttr ".tg[0].tor" -type "double3" 1.510808227529902 -3.2290081270731563 66.850961050407278 ;
	setAttr ".lr" -type "double3" -2.9817700138720465e-16 -7.4544250346801199e-17 -1.272532473628518e-14 ;
	setAttr ".rst" -type "double3" 1.1187244633632205 -0.43539941621753581 -0.081947059913627038 ;
	setAttr ".rsrr" -type "double3" -9.9392333795734874e-17 -4.9696166897867449e-17 
		-1.2731536757147415e-14 ;
	setAttr -k on ".w0";
createNode joint -n "ear1_R_JNT" -p "head_JNT";
	rename -uid "76BA56CE-439C-B1E8-8449-D8B2B093D299";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 1.510808227529856 176.77099187292683 23.14903894959274 ;
	setAttr ".bps" -type "matrix" 0.3925001057986699 -0.91802556418698322 -0.056326995723221765 0
		 -0.91974954629554084 -0.39162205864115585 -0.026323663780139603 0 0.0021069022710314216 0.062138769579337505 -0.99806529559843238 0
		 -0.43539899999999998 3.3431299999999999 -0.082778699999999997 1;
	setAttr ".liw" yes;
createNode joint -n "ear2_R_JNT" -p "ear1_R_JNT";
	rename -uid "B6257D73-4291-E8B6-55C6-6DBFBF6FBC8C";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" -0.93943469396641777 -1.8907033940993421 0.29716208945926187 ;
	setAttr ".bps" -type "matrix" 0.38758303580364312 -0.91749330548532115 -0.089361203812677814 0
		 -0.92147374193802656 -0.38831944049607936 -0.0097033525929486435 0 -0.025797931621801162 0.086104857716803068 -0.99595201701769043 0
		 -0.83484199999999964 4.2857799999999999 -0.15021399999999996 1;
	setAttr ".liw" yes;
createNode joint -n "earTIP_JNT" -p "ear2_R_JNT";
	rename -uid "8D823DD9-485A-2D2C-8524-DE81B31EEB0A";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".t" -type "double3" -0.79968046600848997 0.00041030114752271807 -0.088870086787654567 ;
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" -4.6720897196158573 1.4783167287106322 -67.234776362604549 ;
	setAttr ".bps" -type "matrix" 0.99999966392981798 0.00081673337538240879 7.1322119155937252e-05 0
		 0.00081706079500763534 -0.99998853567882129 -0.0047181482155770338 0 6.7467832378591275e-05 0.0047182049044555002 -0.9999888669333129 0
		 -1.1428700000000001 5.0116699999999996 0.0097527699999999662 1;
	setAttr ".liw" yes;
createNode parentConstraint -n "ear2_R_JNT_parentConstraint1" -p "ear2_R_JNT";
	rename -uid "651C20BC-4629-4175-8E3D-06953131ABBF";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "ear2_R_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -3.1552855717098538e-07 3.636064270118311e-06 
		3.7162334837193001e-07 ;
	setAttr ".tg[0].tor" -type "double3" -179.44179684239023 5.1268586898377206 -67.099009589418372 ;
	setAttr ".lr" -type "double3" 6.3253902429691896e-15 -1.4920497608476911e-15 -1.4082651294683186e-14 ;
	setAttr ".rst" -type "double3" -1.0183597899867038 1.2860695619565377e-07 0.12503835640838384 ;
	setAttr ".rsrr" -type "double3" 6.3277197507925278e-15 -6.969110904818121e-16 -1.4076439273820952e-14 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "ear1_R_JNT_parentConstraint1" -p "ear1_R_JNT";
	rename -uid "0B101F89-4A5F-BB78-03F6-059ACE6499C4";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "ear1_R_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" 4.1621753560372454e-07 2.272491454125003e-06 
		-2.3875779367465988e-08 ;
	setAttr ".tg[0].tor" -type "double3" -178.48919177247015 3.2290081270731648 -66.850961050407264 ;
	setAttr ".lr" -type "double3" 6.4605016967227685e-15 4.9696166897867499e-16 -9.5416640443905503e-15 ;
	setAttr ".rst" -type "double3" 1.118726735854676 0.43539899999999992 -0.081947083789406433 ;
	setAttr ".rsrr" -type "double3" 6.4605016967227685e-15 4.9696166897867499e-16 -9.5416640443905503e-15 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "head_JNT_parentConstraint1" -p "head_JNT";
	rename -uid "A5A1E27D-46EA-9AB6-1923-CB9A77F898D3";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "head_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" 1.5407439555097887e-33 -1.1720233272782821e-07 
		1.6921464338244541e-11 ;
	setAttr ".tg[0].tor" -type "double3" 0 0 90 ;
	setAttr ".rst" -type "double3" 0.53600237509178794 -4.6222318665293654e-33 -1.0842021724855044e-19 ;
	setAttr -k on ".w0";
createNode joint -n "shoulder_L_JNT" -p "chest_JNT";
	rename -uid "2A32D6FF-40AC-4D9B-EC9C-1FA2C0645CA8";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 0 0 -90 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 0.40845635520577289 1.8094612670184995 -0.0008316162105935776 1;
	setAttr ".liw" yes;
createNode joint -n "elbow_L_JNT" -p "shoulder_L_JNT";
	rename -uid "28C13E7E-4B14-A918-2A82-CEA87881D4C7";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 0.85055884512012658 1.8094612670184995 -0.0008316162105935776 1;
	setAttr ".liw" yes;
createNode joint -n "handTIP_L_JNT" -p "elbow_L_JNT";
	rename -uid "8C6929FA-430D-0E12-CDFC-F08EB53A20AC";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".t" -type "double3" 0.65754329718226223 0 0 ;
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 0 0 90 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 1 0 0 -1 0 0 0 0 0 1 0 1.5081021423023888 1.8094612670184995 -0.0008316162105935776 1;
	setAttr ".liw" yes;
createNode parentConstraint -n "elbow_L_JNT_parentConstraint1" -p "elbow_L_JNT";
	rename -uid "11B08F7A-49D5-DE76-96DF-5F95915550DD";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "elbow_L_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" 2.7733499274873452e-08 3.1018438262364612e-08 
		1.6921464121404106e-11 ;
	setAttr ".rst" -type "double3" 0.44210248991435358 0 0 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "shoulder_L_JNT_parentConstraint1" -p "shoulder_L_JNT";
	rename -uid "052E23F1-471D-4306-802B-1694262A010E";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "shoulder_L_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -1.2755535516717487e-10 3.1018438484409216e-08 
		1.6921464012983889e-11 ;
	setAttr ".rst" -type "double3" 0.12106037796496305 -0.40845635520577289 -1.0842021724855044e-19 ;
	setAttr -k on ".w0";
createNode joint -n "shoulder_R_JNT" -p "chest_JNT";
	rename -uid "6F1555CD-406A-B997-E97F-74BFAAF90900";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" -180 -7.0622500768802503e-31 -90 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 1 0 0 0 0 -1 -1.2246467991473532e-16 0 0 1.2246467991473532e-16 -1 0
		 -0.40845599999999999 1.8094600000000001 -0.00083161599999999997 1;
	setAttr ".liw" yes;
createNode joint -n "elbow_R_JNT" -p "shoulder_R_JNT";
	rename -uid "712A7C12-4716-07A3-0202-969F12F5CCDB";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 1 0 0 0 0 -1 -1.2246467991473532e-16 0 0 1.2246467991473532e-16 -1 0
		 -0.85055899999999995 1.8094600000000001 -0.00083161599999999997 1;
	setAttr ".liw" yes;
createNode joint -n "handTIP_R_JNT" -p "elbow_R_JNT";
	rename -uid "D4C36787-4398-89DA-AF5A-C2A67567EFA2";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".t" -type "double3" -0.657541 0 0 ;
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 7.016709298534876e-15 7.0167092985348775e-15 90 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 -1 -1.2246467991473532e-16 0 -1 0 0 0 0 1.2246467991473532e-16 -1 0
		 -1.5081 1.8094600000000001 -0.00083161599999999997 1;
	setAttr ".liw" yes;
createNode parentConstraint -n "elbow_R_JNT_parentConstraint1" -p "elbow_R_JNT";
	rename -uid "E62E5DBC-45C1-A266-3FC1-B9B668CF1ADB";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "elbow_R_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -1.8261337286773482e-07 -1.2360000607447574e-06 
		2.2751498753546817e-10 ;
	setAttr ".tg[0].tor" -type "double3" -180 0 0 ;
	setAttr ".rst" -type "double3" -0.44210299999999991 0 1.0842021724855044e-19 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "shoulder_R_JNT_parentConstraint1" -p "shoulder_R_JNT";
	rename -uid "B2E232ED-4AB0-AB58-2D12-DC8175CC46D6";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "shoulder_R_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" 3.5533332809478679e-07 -1.2360000605227128e-06 
		2.2751504163715658e-10 ;
	setAttr ".tg[0].tor" -type "double3" 180 0 0 ;
	setAttr ".lr" -type "double3" -1.4033418597069752e-14 0 0 ;
	setAttr ".rst" -type "double3" 0.12105911094646382 0.40845600000000015 2.1059357751575247e-10 ;
	setAttr ".rsrr" -type "double3" -1.4033418597069752e-14 0 0 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "chest_JNT_parentConstraint1" -p "chest_JNT";
	rename -uid "2EBC9F19-42D4-AFE4-8555-718D877E8BBF";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "chest_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -3.0814879110195774e-33 2.4452400770869076e-08 
		1.6921464446664758e-11 ;
	setAttr ".tg[0].tor" -type "double3" 0 0 90 ;
	setAttr ".rst" -type "double3" 0.87912925542850551 1.5407439555097887e-33 0 ;
	setAttr -k on ".w0";
createNode joint -n "thigh_L_JNT" -p "pelvis_JNT";
	rename -uid "374523C4-42CD-30CC-3416-0CA33D5B4C91";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 0 180 0 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 -1 -1.2246467991473532e-16 0 -1 0 0 0 0 1.2246467991473532e-16 -1 0
		 0.29430170637130593 0.54792957913111406 0.00066644006628668873 1;
	setAttr ".liw" yes;
createNode joint -n "foot_L_JNT" -p "thigh_L_JNT";
	rename -uid "EEE41443-4324-1E49-013B-7BA59583EA12";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 -1 -1.2246467991473532e-16 0 -1 0 0 0 0 1.2246467991473532e-16 -1 0
		 0.29430170637130593 0.30389837187225333 0.00066644006628668873 1;
	setAttr ".liw" yes;
createNode joint -n "footTIP_L_JNT" -p "foot_L_JNT";
	rename -uid "388040C3-4BBE-838D-CBED-56A905A26647";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".t" -type "double3" 0.1372675540831092 0 -0.33706266229804505 ;
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 0 180 0 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 1 2.4492935982947064e-16 0 -1 0 0 0 0 -2.4492935982947064e-16 1 0
		 0.29430170637130593 0.1666308177891441 0.33772910236433173 1;
	setAttr ".liw" yes;
createNode parentConstraint -n "foot_L_JNT_parentConstraint1" -p "foot_L_JNT";
	rename -uid "8D3717A2-4064-C86E-3027-2280212B6F75";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "foot_L_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -1.2102128632562881e-08 7.5667573873339222e-09 
		9.1840973845055962e-12 ;
	setAttr ".tg[0].tor" -type "double3" 180 7.016709298534876e-15 -90 ;
	setAttr ".lr" -type "double3" -7.016709298534876e-15 7.016709298534876e-15 -4.2964952914991028e-31 ;
	setAttr ".rst" -type "double3" 0.24403120725886074 0 -2.9923979960599922e-17 ;
	setAttr ".rsrr" -type "double3" -7.016709298534876e-15 7.016709298534876e-15 -4.2964952914991028e-31 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "thigh_L_JNT_parentConstraint1" -p "thigh_L_JNT";
	rename -uid "D23B2A92-41F3-F6EA-C0D3-0CB56BA69C1C";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "leg_L_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -1.2102128632562881e-08 -5.848896700300088e-09 
		9.1840974929258135e-12 ;
	setAttr ".tg[0].tor" -type "double3" 180 7.016709298534876e-15 -90 ;
	setAttr ".lr" -type "double3" -7.016709298534876e-15 7.016709298534876e-15 0 ;
	setAttr ".rst" -type "double3" -0.26134205449391645 -0.29430170637130593 0.0014980562768802663 ;
	setAttr ".rsrr" -type "double3" -7.016709298534876e-15 7.016709298534876e-15 0 ;
	setAttr -k on ".w0";
createNode joint -n "thigh_R_JNT" -p "pelvis_JNT";
	rename -uid "B27C2563-4F27-3CCD-1158-C5A9EFBC3FE8";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 1 0 0 -1 0 0 0 0 0 1 0 -0.29430200000000001 0.54793000000000003 0.00066643999999999991 1;
	setAttr ".liw" yes;
createNode joint -n "foot_R_JNT" -p "thigh_R_JNT";
	rename -uid "C1A828F6-4FC6-8DA4-9DAE-C4B2E9FFBC69";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 1 0 0 -1 0 0 0 0 0 1 0 -0.29430200000000001 0.303898 0.00066644000000000002 1;
	setAttr ".liw" yes;
createNode joint -n "footTIP_R_JNT" -p "foot_R_JNT";
	rename -uid "9858ECE6-4D61-FA60-4C60-12A17D650A5E";
	addAttr -ci true -sn "liw" -ln "lockInfluenceWeights" -min 0 -max 1 -at "bool";
	setAttr ".t" -type "double3" -0.137267 0 0.33706256000000001 ;
	setAttr ".mnrl" -type "double3" -360 -360 -360 ;
	setAttr ".mxrl" -type "double3" 360 360 360 ;
	setAttr ".jo" -type "double3" 0 180 0 ;
	setAttr ".ssc" no;
	setAttr ".bps" -type "matrix" 0 -1 -1.2246467991473532e-16 0 -1 0 0 0 0 1.2246467991473532e-16 -1 0
		 -0.29430200000000001 0.166631 0.337729 1;
	setAttr ".liw" yes;
createNode parentConstraint -n "foot_R_JNT_parentConstraint1" -p "foot_R_JNT";
	rename -uid "B53892A4-414B-ED5C-C6DC-5BB8E12D9494";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "foot_R_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -2.815265654487753e-07 -3.6430549582577854e-07 
		-5.7102591318608509e-11 ;
	setAttr ".tg[0].tor" -type "double3" 0 0 90 ;
	setAttr ".rst" -type "double3" -0.24403200000000003 0 1.0842021724855044e-19 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "thigh_R_JNT_parentConstraint1" -p "thigh_R_JNT";
	rename -uid "A3B4FC75-4709-396A-C40A-2D86C4E8EF11";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "leg_R_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" -2.815265654487753e-07 4.1501998937487627e-07 
		-5.7102591318608509e-11 ;
	setAttr ".tg[0].tor" -type "double3" 0 -1.4033418597069752e-14 90 ;
	setAttr ".rst" -type "double3" -0.26134163362503038 0.294302 0.0014980562105935775 ;
	setAttr -k on ".w0";
createNode parentConstraint -n "pelvis_JNT_parentConstraint1" -p "pelvis_JNT";
	rename -uid "04074B73-42FB-75A1-5AE1-2BAA17EDB960";
	addAttr -dcb 0 -ci true -k true -sn "w0" -ln "hip_CTRLW0" -dv 1 -min 0 -at "double";
	setAttr -k on ".nds";
	setAttr -k off ".v";
	setAttr -k off ".tx";
	setAttr -k off ".ty";
	setAttr -k off ".tz";
	setAttr -k off ".rx";
	setAttr -k off ".ry";
	setAttr -k off ".rz";
	setAttr -k off ".sx";
	setAttr -k off ".sy";
	setAttr -k off ".sz";
	setAttr ".erp" yes;
	setAttr ".tg[0].tot" -type "double3" 0 0 1.6921464229824323e-11 ;
	setAttr ".tg[0].tor" -type "double3" 0 0 90 ;
	setAttr ".rst" -type "double3" 1.2554342668302078e-17 0.80927163362503052 -0.0008316162105935776 ;
	setAttr -k on ".w0";
createNode transform -n "GEO_GRP" -p "root_CTRL";
	rename -uid "122F79F8-4FB5-8F67-DAF2-58B5718A024E";
createNode transform -n "rabbit_GEO" -p "GEO_GRP";
	rename -uid "E44C10C4-4154-147E-447C-F19ED40FF083";
	setAttr -l on ".tx";
	setAttr -l on ".ty";
	setAttr -l on ".tz";
	setAttr -l on ".rx";
	setAttr -l on ".ry";
	setAttr -l on ".rz";
	setAttr -l on ".sx";
	setAttr -l on ".sy";
	setAttr -l on ".sz";
	setAttr ".rp" -type "double3" 0 2.5099683701992035 0.17151844501495361 ;
	setAttr ".sp" -type "double3" 0 2.5099683701992035 0.17151844501495361 ;
	setAttr ".it" no;
createNode mesh -n "rabbit_GEOShape" -p "rabbit_GEO";
	rename -uid "C13AED1B-4051-862D-AF5A-1FA34577C98D";
	setAttr -k off ".v";
	setAttr -s 2 ".iog[0].og";
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr ".pv" -type "double2" 0.76819780468940735 0.93847781419754028 ;
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
	setAttr ".vcs" 2;
createNode mesh -n "rabbit_GEOShapeOrig" -p "rabbit_GEO";
	rename -uid "FF159862-4CF1-0EB5-DFCC-068FF6F630EF";
	setAttr -k off ".v";
	setAttr ".io" yes;
	setAttr ".vir" yes;
	setAttr ".vif" yes;
	setAttr ".uvst[0].uvsn" -type "string" "map1";
	setAttr -s 599 ".uvst[0].uvsp";
	setAttr ".uvst[0].uvsp[0:249]" -type "float2" 0.36182141 0.37327486 0.33759278
		 0.53330004 0.33679599 0.083448485 0.35717386 0.24288401 0.23247883 0.36740428 0.22674866
		 0.57384551 0.2408227 0.056143824 0.23534541 0.26199591 0.27043566 0.3689031 0.26446295
		 0.56501293 0.27661076 0.062670887 0.282424 0.26385051 0.23890033 0.12854891 0.27139187
		 0.12984708 0.36476916 0.13706118 0.36931503 0.48105019 0.24555129 0.48274601 0.22936705
		 0.47968912 0.23736979 0.18607488 0.26321894 0.18712676 0.36348677 0.18802825 0.38675955
		 0.43359345 0.25754985 0.42207938 0.2308065 0.4278239 0.26839358 0.38196373 0.32709551
		 0.47859591 0.32634974 0.4383359 0.33754367 0.40851349 0.23379162 0.31975806 0.28076959
		 0.31760919 0.37465963 0.27999857 0.42515689 0.19816133 0.42136726 0.12479639 0.37797683
		 0.061685193 0.28618318 0.010240974 0.24213251 0.0067312438 0.31884807 0.13633505
		 0.31276578 0.18751708 0.24042296 0.42770928 0.23125201 0.41175127 0.10301043 0.36611873
		 0.095267996 0.27253246 0.049202159 0.18814319 0.074768305 0.42495233 0.19581476 0.37996799
		 0.19449678 0.36681807 0.19795862 0.0078956205 0.18958309 0.56293583 0.21150151 0.185745
		 0.18823168 0.2613185 0.18700351 0.3150565 0.1348238 0.43303728 0.12529925 0.40264404
		 0.10356572 0.054384056 0.11832637 0.5272215 0.16200253 0.18349314 0.11470679 0.23640668
		 0.20638534 0.12811412 0.20473871 0.060757983 0.14353529 0.078303277 0.15865037 0.1320654
		 0.089553446 0.47329795 0.056885615 0.11509335 0.13184577 0.47318923 0.21303752 0.48184645
		 0.22121093 0.42717904 0.20442176 0.42061371 0.11132479 0.18130223 0.11275384 0.13034651
		 0.23206818 0.38226688 0.18215212 0.60050654 0.22590518 0.60412204 0.26979193 0.60293901
		 0.088160396 0.55918491 0.36593407 0.56689084 0.015840963 0.42601645 0.030749366 0.50415254
		 0.072857648 0.33847958 0.44553614 0.43793136 0.42631269 0.51512587 0.39346075 0.34735048
		 0.99088931 0.25859016 0.99493957 0.29271513 0.98846072 0.29183495 0.98338628 0.25532228
		 0.90890056 0.17413713 0.86881405 0.1751571 0.86882371 0.16628303 0.90936172 0.16587533
		 0.91162729 0.28998178 0.86868006 0.29091328 0.86870712 0.26805612 0.91451818 0.26570529
		 0.86858976 0.37059999 0.8686012 0.3597455 0.9248296 0.35540518 0.93052143 0.3627477
		 0.86887199 0.1190932 0.86892271 0.066147029 0.92080665 0.069914728 0.9180603 0.11828708
		 0.95258474 0.16487139 0.95294482 0.15760204 0.95402318 0.28983814 0.95342922 0.2574051
		 0.97685587 0.32844204 0.98200637 0.33163595 0.96455973 0.079088628 0.96271974 0.10723677
		 0.92127669 0.34632075 0.8686139 0.34825724 0.86865276 0.31414852 0.91396093 0.31407565
		 0.97040468 0.32776812 0.95286703 0.31475663 0.9759385 0.25433731 0.98198068 0.29097825
		 0.9223237 0.2247088 0.86875278 0.22828059 0.92822117 0.21703216 0.86876321 0.21642199
		 0.9849627 0.18639781 0.99019587 0.18157241 0.86896902 0.015402708 0.92314422 0.02173065
		 0.97047895 0.04801308 0.74654937 0.25830454 0.75406021 0.25505507 0.74889779 0.29155537
		 0.74241662 0.29241958 0.82873058 0.1740516 0.8282873 0.16578935 0.82573479 0.28988552
		 0.82290155 0.26560259 0.8066721 0.36260828 0.81238055 0.35527903 0.81968516 0.1181866
		 0.81703067 0.06981276 0.78506595 0.16469474 0.78472167 0.15742554 0.78333974 0.28964227
		 0.78401101 0.25721091 0.75525671 0.33137265 0.76041502 0.32819134 0.77504706 0.10705127
		 0.77326 0.078903817 0.81595427 0.34620267 0.82334465 0.31397396 0.76686811 0.32753298
		 0.78443694 0.31456357 0.7615099 0.25408849 0.75537974 0.29071462 0.81519324 0.22459055
		 0.80931431 0.21690083 0.75264025 0.18614683 0.74741817 0.18130949 0.81478208 0.021626372
		 0.767398 0.047819052 0.50192577 0.33763179 0.55450386 0.33707979 0.55442917 0.45976886
		 0.51113349 0.45960447 0.55446005 0.22240576 0.55448908 0.23964189 0.51791936 0.254648
		 0.5175938 0.23419258 0.55398506 0.029261649 0.55426389 0.15205708 0.5033316 0.15168408
		 0.51115471 0.029402673 0.59681553 0.029208243 0.51112354 0.012019873 0.55394548 0.011886656
		 0.59676749 0.011824012 0.49361271 0.4603115 0.48711652 0.33674368 0.61509001 0.46030661
		 0.59769446 0.45959106 0.60889304 0.33770189 0.62190181 0.33689496 0.60519344 0.15145662
		 0.59107238 0.25456318 0.59130317 0.23405883 0.60846597 0.25317952 0.50068879 0.25332823
		 0.50192636 0.33763167 0.51113373 0.45960435 0.55442989 0.45976886 0.55450416 0.3370795
		 0.55446005 0.22240564 0.5175944 0.23419258 0.51791978 0.25464794 0.55448937 0.23964201
		 0.55398506 0.029261827 0.51115507 0.029402912 0.50333166 0.15168387 0.55426389 0.15205669
		 0.59681547 0.029208302 0.59676778 0.011824071 0.55394542 0.011886895 0.51112384 0.012019634
		 0.48711652 0.33674356 0.49361324 0.46031138 0.61509007 0.46030661 0.62190181 0.33689508
		 0.60889345 0.33770189 0.59769458 0.45959088 0.6051935 0.15145642 0.59107274 0.2545633
		 0.59130347 0.23405895 0.60846621 0.25317928 0.50068915 0.25332817 0.83263385 0.73450685
		 0.83406115 0.66640401 0.861687 0.68640912 0.85875261 0.75665998 0.63025808 0.51510006
		 0.59526038 0.52788103 0.58013642 0.52731735 0.58978975 0.50158745 0.96948016 0.74663091
		 0.96628237 0.67993248 0.99377477 0.66502023 0.99545979 0.73258454 0.91217613 0.40309271
		 0.91276729 0.46529663 0.86997533 0.46884543 0.8821398 0.40426761 0.85896248 0.55250847
		 0.91365397 0.55236781 0.91384822 0.58131868 0.86203092 0.58103275 0.95133376 0.40563786
		 0.96099794 0.46885222 0.96554345 0.57988483 0.96491623 0.55246055 0.56316757 0.52314717
		 0.55306256 0.50139862 0.54535872 0.54407817 0.91415077 0.6022284 0.95189685 0.60257769
		 0.87905639 0.60278326 0.61260802 0.55636239 0.50585043 0.54686123 0.45852739 0.52105683
		 0.48785594 0.47019771 0.62102026 0.55418307 0.68282717 0.53317702 0.68872368 0.56393933
		 0.47408238 0.59499222;
	setAttr ".uvst[0].uvsp[250:499]" 0.4245621 0.56749254 0.57174462 0.5784657
		 0.5680936 0.589867 0.53254503 0.57923537 0.53793097 0.57025021 0.62035197 0.58756989
		 0.68874633 0.56406265 0.62976587 0.6474542 0.69663388 0.65637451 0.68611407 0.75918394
		 0.61283481 0.75714397 0.55285907 0.63346761 0.51821154 0.73014772 0.46199197 0.70374727
		 0.50996608 0.61585158 0.41874734 0.6801244 0.37024361 0.65284991 0.42161781 0.57208908
		 0.67076069 0.48093376 0.71278828 0.47508869 0.70980465 0.50789833 0.67463255 0.49458012
		 0.59699851 0.58632153 0.59603101 0.59614462 0.59234381 0.6368885 0.56545979 0.74605286
		 0.39242357 0.72973996 0.34331822 0.70210457 0.49483013 0.78032446 0.43656051 0.75320488
		 0.67663878 0.80914539 0.60174817 0.817128 0.54599947 0.79889846 0.68858284 0.89460194
		 0.56658769 0.90552908 0.5738914 0.8582024 0.68064409 0.8540616 0.59401762 0.95650953
		 0.68974549 0.95414186 0.68647581 0.98956484 0.63367242 0.97790414 0.62353235 0.48118791
		 0.62700886 0.45856583 0.67574137 0.46605232 0.67480862 0.40964118 0.70560992 0.43971863
		 0.88871431 0.68068635 0.88954538 0.73552001 0.91353607 0.74012589 0.913782 0.68044549
		 0.93723279 0.7402432 0.93869054 0.68361193 0.5270732 0.5622322 0.69652683 0.65508157
		 0.86256343 0.66542751 0.9660936 0.67599446 0.83263385 0.73450685 0.85875261 0.75665998
		 0.861687 0.68640912 0.83406115 0.66640401 0.63025808 0.51510006 0.58978975 0.50158745
		 0.58013642 0.52731735 0.59526038 0.52788103 0.96948016 0.74663091 0.99545979 0.73258454
		 0.99377477 0.66502023 0.96628237 0.67993248 0.91217613 0.40309271 0.8821398 0.40426761
		 0.86997533 0.46884543 0.91276729 0.46529663 0.85896248 0.55250847 0.86203092 0.58103275
		 0.91384822 0.58131868 0.91365397 0.55236781 0.95133376 0.40563786 0.96099794 0.46885222
		 0.96554345 0.57988483 0.96491623 0.55246055 0.55306256 0.50139862 0.56316757 0.52314717
		 0.54535872 0.54407817 0.91415077 0.6022284 0.95189685 0.60257769 0.87905639 0.60278326
		 0.61260802 0.55636239 0.48785594 0.47019771 0.45852739 0.52105683 0.50585043 0.54686123
		 0.62102026 0.55418307 0.68872368 0.56393933 0.68282717 0.53317702 0.4245621 0.56749254
		 0.47408238 0.59499222 0.57174462 0.5784657 0.53793097 0.57025021 0.53254503 0.57923537
		 0.5680936 0.589867 0.62035197 0.58756989 0.68874633 0.56406265 0.62976587 0.6474542
		 0.61283481 0.75714397 0.68611407 0.75918394 0.69663388 0.65637451 0.55285907 0.63346761
		 0.50996608 0.61585158 0.46199197 0.70374727 0.51821154 0.73014772 0.42161781 0.57208908
		 0.37024361 0.65284991 0.41874734 0.6801244 0.67076069 0.48093376 0.67463255 0.49458012
		 0.70980465 0.50789833 0.71278828 0.47508869 0.59699851 0.58632153 0.59603101 0.59614462
		 0.59234381 0.6368885 0.56545979 0.74605286 0.34331822 0.70210457 0.39242357 0.72973996
		 0.43656051 0.75320488 0.49483013 0.78032446 0.60174817 0.817128 0.67663878 0.80914539
		 0.54599947 0.79889846 0.68858284 0.89460194 0.68064409 0.8540616 0.5738914 0.8582024
		 0.56658769 0.90552908 0.59401762 0.95650953 0.63367242 0.97790414 0.68647581 0.98956484
		 0.68974549 0.95414186 0.62353235 0.48118791 0.67574137 0.46605232 0.62700886 0.45856583
		 0.70560992 0.43971863 0.67480862 0.40964118 0.88954538 0.73552001 0.88871431 0.68068635
		 0.91353607 0.74012589 0.913782 0.68044549 0.93723279 0.7402432 0.93869054 0.68361193
		 0.5270732 0.5622322 0.69652683 0.65508157 0.86256343 0.66542751 0.9660936 0.67599446
		 0.15136917 0.78542292 0.13106151 0.78475887 0.13474183 0.76617771 0.16032121 0.76704556
		 0.16643433 0.79089689 0.18530731 0.77437484 0.06525372 0.78681821 0.048278984 0.79084074
		 0.03021976 0.77000457 0.058566682 0.76691967 0.086558804 0.78587413 0.084975287 0.76693088
		 0.1089958 0.78528571 0.11003399 0.76657265 0.13634762 0.74777687 0.16416892 0.7486155
		 0.19944182 0.75298369 0.021653052 0.74419653 0.05700893 0.74814475 0.084888712 0.74877435
		 0.110633 0.7482419 0.13628592 0.72960901 0.162714 0.72979879 0.19110173 0.72703379
		 0.035590824 0.72269422 0.060716603 0.7297371 0.086394638 0.73041362 0.11118378 0.72993225
		 0.13465333 0.71066922 0.156091 0.70983398 0.17316577 0.70597249 0.054301385 0.70600682
		 0.069475368 0.71133018 0.089884475 0.71186274 0.11206825 0.71126044 0.15136299 0.78541964
		 0.1310555 0.78475243 0.13473888 0.76617193 0.16031808 0.76704401 0.16642731 0.7908963
		 0.18530306 0.77437723 0.06524758 0.78680092 0.048271731 0.79082066 0.03021602 0.76998138
		 0.058563355 0.76690137 0.086552426 0.78586036 0.084972039 0.76691681 0.10898951 0.78527576
		 0.11003093 0.76656282 0.13634762 0.7477712 0.16416875 0.74861437 0.19944108 0.75298828
		 0.021653797 0.74417198 0.057008743 0.74812621 0.084888428 0.74876052 0.110633 0.74823225
		 0.13628887 0.72960353 0.16271695 0.72979742 0.19110513 0.72703707 0.035594884 0.72267199
		 0.06071965 0.72971916 0.08639732 0.73039985 0.11118655 0.72992253 0.13465934 0.7106635
		 0.15609729 0.70983183 0.17317252 0.70597297 0.054308224 0.70598769 0.069481283 0.71131361
		 0.089890301 0.71184963 0.11207426 0.71125096 0.15136983 0.78542304 0.13106224 0.78475958
		 0.13474211 0.76617849 0.16032158 0.76704568 0.16643517 0.79089695 0.18530786 0.77437437
		 0.065254509 0.78682029 0.04827977 0.79084331 0.030219894 0.77000737 0.058567099 0.76692194
		 0.086559445 0.7858758 0.084975645 0.76693267 0.10899645 0.78528696 0.11003426 0.76657367
		 0.13634753 0.74777752 0.16416875 0.74861532 0.19944182 0.75298285 0.021653052 0.7441994
		 0.05700884 0.74814713 0.084888622 0.74877608 0.110633 0.74824309 0.13628535 0.72960967
		 0.16271336 0.72979856 0.19110116 0.72703284 0.035590265 0.72269696 0.060716279 0.72973925
		 0.086394086 0.73041528 0.11118323 0.72993326 0.13465232 0.71067011 0.15609011 0.70983428;
	setAttr ".uvst[0].uvsp[500:598]" 0.17316476 0.70597231 0.054300372 0.70600915
		 0.069474265 0.7113322 0.089883551 0.71186429 0.11206742 0.71126157 0.88440311 0.82075864
		 0.89527547 0.79438186 0.94982415 0.83365381 0.93047053 0.85148412 0.79928249 0.90332824
		 0.7886194 0.92368984 0.74387389 0.92798895 0.73198169 0.90667635 0.79761368 0.78715181
		 0.79520297 0.81532764 0.73565668 0.81305844 0.739223 0.78364158 0.85688722 0.81663024
		 0.86203432 0.7890045 0.82802272 0.78820783 0.82519752 0.81641382 0.79454154 0.88086313
		 0.73407078 0.88062727 0.82949525 0.92335069 0.80476975 0.95325553 0.83286721 0.89191306
		 0.79510731 0.84406102 0.73624396 0.84218383 0.73162585 0.95326579 0.85444969 0.85666221
		 0.82493919 0.84583998 0.84825766 0.84102577 0.87227339 0.84111905 0.72968954 0.72701156
		 0.81469381 0.73259002 0.8126905 0.76319927 0.72753119 0.75772172 0.9156884 0.87076837
		 0.96779162 0.86107743 0.94439906 0.87576348 0.9944123 0.91794032 0.96937978 0.93392026
		 0.95681584 0.90533018 0.98161441 0.89145333 0.7042076 0.92738473 0.69661939 0.89719087
		 0.9090603 0.89178538 0.92543274 0.89224857 0.92924619 0.91565681 0.94064015 0.94236243
		 0.80277503 0.70774251 0.7448619 0.70392132 0.88440305 0.82075882 0.93047035 0.85148424
		 0.94982392 0.83365399 0.89527529 0.79438186 0.79928219 0.90332836 0.73198169 0.90667629
		 0.74387389 0.92798895 0.7886191 0.92368984 0.79761374 0.78715205 0.73922306 0.7836417
		 0.73565662 0.81305844 0.79520285 0.81532764 0.86203414 0.78900456 0.85688692 0.81663048
		 0.8251977 0.81641376 0.82802236 0.78820789 0.79454124 0.88086313 0.73407078 0.88062739
		 0.80476964 0.95325553 0.82949501 0.92335075 0.83286709 0.89191318 0.79510707 0.8440612
		 0.73624414 0.84218401 0.73162603 0.95326579 0.85444957 0.85666239 0.87227315 0.84111929
		 0.84825742 0.84102577 0.82493889 0.8458401 0.72968942 0.72701174 0.72753114 0.7577219
		 0.81269038 0.76319927 0.81469369 0.73258996 0.91568822 0.87076843 0.94439882 0.87576377
		 0.96779126 0.86107755 0.99441212 0.91794026 0.98161423 0.8914535 0.95681554 0.90533
		 0.96937948 0.93392026 0.70420742 0.92738479 0.69661939 0.89719105 0.90906006 0.8917855
		 0.92924595 0.91565675 0.92543238 0.89224881 0.94063985 0.94236255 0.80277503 0.70774263
		 0.74486166 0.70392132;
	setAttr ".cuvs" -type "string" "map1";
	setAttr ".dcc" -type "string" "Ambient+Diffuse";
	setAttr ".covm[0]"  0 1 1;
	setAttr ".cdvm[0]"  0 1 1;
	setAttr -s 435 ".vt";
	setAttr ".vt[0:165]"  0.91492659 2.11125803 0.37759548 0.83642054 3.15802836 0.32685733
		 0.7400744 3.17690706 -0.37204605 0.91492659 2.11125803 -0.22887255 -8.3899027e-19 2.054889679 0.6696555
		 -1.890783e-17 3.40345073 0.21173128 7.2819025e-18 3.34201217 -0.39704734 -2.035354e-17 2.04361248 -0.24171753
		 0.27744427 2.15537739 0.59181249 0.27744427 3.40345073 0.28876966 0.27744427 3.31121039 -0.40571636
		 0.34049278 2.086214066 -0.20030145 -6.623641e-19 2.90183687 -0.79435599 0.28970006 2.90183687 -0.7798121
		 1.07612896 2.85373998 -0.50988519 1.19210386 2.85373998 0.48858684 0.13796338 2.90183687 0.72313792
		 -2.7574324e-17 2.90183687 0.76603442 -1.1025501e-17 2.44161677 -0.71814454 0.22220705 2.44161677 -0.71020681
		 1.039757013 2.44642639 -0.48287815 1.27195704 2.44642639 0.45929569 0.2101087 2.4611845 0.82648963
		 2.7429872e-18 2.50827479 1.002792716 0.27744427 2.21150684 0.6696555 0.86440092 2.86812997 0.6922043
		 0.8692562 2.54262018 0.85457218 0.90411556 2.26200843 0.73878175 -7.8221061e-18 1.97189891 0.25937968
		 0.28706363 2.026592731 0.21004106 0.95340407 2.01025486 0.074361458 1.36334109 2.44642639 -0.07353574
		 1.25462985 2.90183687 -0.058890179 0.85565925 3.13863707 -0.032275662 0.27744427 3.46124077 -0.020787418
		 2.2574716e-18 3.47559643 -0.020787418 0.70964026 2.86943817 -0.71210867 0.64840066 2.44480848 -0.65527827
		 0.10505435 2.50514221 0.96425813 -1.9881349e-17 2.38649988 0.88072473 -0.91492659 2.11125803 0.37759548
		 -0.83642054 3.15802836 0.32685733 -0.7400744 3.17690706 -0.37204605 -0.91492659 2.11125803 -0.22887255
		 -0.27744427 2.15537739 0.59181249 -0.27744427 3.40345073 0.28876966 -0.27744427 3.31121039 -0.40571636
		 -0.34049278 2.086214066 -0.20030145 -0.28970006 2.90183687 -0.7798121 -1.07612896 2.85373998 -0.50988519
		 -1.19210386 2.85373998 0.48858684 -0.13796338 2.90183687 0.72313792 -0.22220705 2.44161677 -0.71020681
		 -1.039757013 2.44642639 -0.48287815 -1.27195704 2.44642639 0.45929569 -0.2101087 2.4611845 0.82648963
		 -0.27744427 2.21150684 0.6696555 -0.86440092 2.86812997 0.6922043 -0.8692562 2.54262018 0.85457218
		 -0.90411556 2.26200843 0.73878175 -0.28706363 2.026592731 0.21004106 -0.95340407 2.01025486 0.074361458
		 -1.36334109 2.44642639 -0.07353574 -1.25462985 2.90183687 -0.058890179 -0.85565925 3.13863707 -0.032275662
		 -0.27744427 3.46124077 -0.020787418 -0.70964026 2.86943817 -0.71210867 -0.64840066 2.44480848 -0.65527827
		 -0.10505435 2.50514221 0.96425813 -7.2395079e-18 2.14072943 0.77784395 0.92133975 3.099333525 0.26718932
		 0.87433112 3.43246555 0.36000317 0.87433112 3.43246555 -0.26962945 0.95450109 3.099333525 -0.31150889
		 0 3.29118395 0.59037375 -1.4012805e-17 3.73588848 0.23406923 -3.1253901e-17 3.73588848 -0.23406923
		 0 3.29118395 -0.54258901 0.49881637 3.23936343 0.50283706 0.50277805 3.66277623 0.2445918
		 0.50277805 3.66277623 -0.2445918 0.49881637 3.23936343 -0.5072729 -2.5233277e-17 3.51249242 -0.5
		 0.50277805 3.41618514 -0.5 0.97155619 3.21459174 -0.36966115 0.93780231 3.21459174 0.32534158
		 0.50277805 3.41618514 0.59037375 1.2586284e-17 3.51249242 0.59037375 0 3.4018383 -0.54258907
		 0.50079703 3.32777405 -0.55104119 0.99532789 3.15696335 -0.34058499 0.9607482 3.15696335 0.29626545
		 0.50079703 3.32777405 0.54660541 0 3.4018383 0.59037375 0.50079703 3.32777405 1.059212327
		 -5.5511151e-17 3.4018383 1.13739288 0 3.29118395 1.13739288 0.49881637 3.23936343 1.05921185
		 0.9607482 3.15696335 0.71913064 0.92133975 3.099333525 0.71913064 0 3.29118395 0
		 0.49881637 3.23936343 -0.013720751 0.92133975 3.099333525 0 0.9607482 3.15696335 0
		 1.0001565218 3.21459174 0 0.90836883 3.5002265 0 0.50277805 3.75939131 0 -2.189396e-17 3.79718661 0
		 -0.92133975 3.099333525 0.26718932 -0.87433112 3.43246555 0.36000317 -0.87433112 3.43246555 -0.26962945
		 -0.95450109 3.099333525 -0.31150889 -0.49881637 3.23936343 0.50283706 -0.50277805 3.66277623 0.2445918
		 -0.50277805 3.66277623 -0.2445918 -0.49881637 3.23936343 -0.5072729 -0.50277805 3.41618514 -0.5
		 -0.97155619 3.21459174 -0.36966115 -0.93780231 3.21459174 0.32534158 -0.50277805 3.41618514 0.59037375
		 -0.50079703 3.32777405 -0.55104119 -0.99532789 3.15696335 -0.34058499 -0.9607482 3.15696335 0.29626545
		 -0.50079703 3.32777405 0.54660541 -0.50079703 3.32777405 1.059212327 -0.49881637 3.23936343 1.05921185
		 -0.9607482 3.15696335 0.71913064 -0.92133975 3.099333525 0.71913064 -0.49881637 3.23936343 -0.013720751
		 -0.92133975 3.099333525 0 -0.9607482 3.15696335 0 -1.0001565218 3.21459174 0 -0.90836883 3.5002265 0
		 -0.50277805 3.75939131 0 0.12097561 3.48146701 0.072222233 0.77516282 3.20478868 0.072222233
		 0.82327241 5.012721062 0.072222233 1.38469744 4.77527618 0.072222233 0.82327241 5.012721062 -0.072222203
		 1.38469744 4.77527618 -0.072222203 0.12097561 3.48146701 -0.072222203 0.77516282 3.20478868 -0.072222203
		 0.4480691 3.34312773 -0.012483388 1.15417361 5.012666225 0.072222233 1.15417361 5.012666225 -0.072222203
		 0.4480691 3.34312773 -0.15692782 0.45351768 4.44064713 0.072222233 0.45351768 4.44064713 -0.072222203
		 0.84674615 4.28577375 -0.22878343 1.23176837 4.11149788 -0.072222203 1.23176837 4.11149788 0.072222233
		 0.84674615 4.28577375 -0.084338993 -0.12097561 3.48146701 0.072222233 -0.77516282 3.20478868 0.072222233
		 -0.82327241 5.012721062 0.072222233 -1.38469744 4.77527618 0.072222233 -0.82327241 5.012721062 -0.072222203
		 -1.38469744 4.77527618 -0.072222203 -0.12097561 3.48146701 -0.072222203 -0.77516282 3.20478868 -0.072222203
		 -0.4480691 3.34312773 0.072222233 -1.15417361 5.012666225 0.072222233 -1.15417361 5.012666225 -0.072222203
		 -0.4480691 3.34312773 -0.072222203 -0.45351768 4.44064713 0.072222233 -0.45351768 4.44064713 -0.072222203;
	setAttr ".vt[166:331]" -0.84674615 4.28577375 -0.072222203 -1.23176837 4.11149788 -0.072222203
		 -1.23176837 4.11149788 0.072222233 -0.84674615 4.28577375 0.072222233 0.51294464 1.67191553 0.16545029
		 1.38533735 1.67417932 0.29571429 0.25893268 1.95496762 0.10384056 1.34280539 1.95796573 0.2100807
		 0.34301886 1.95590782 -0.09976878 1.3427527 1.95935607 -0.20625122 0.52370834 1.67321277 -0.21289755
		 1.38526273 1.67614806 -0.29378909 0.35158592 1.9555943 0.0020300017 0.51317346 1.67246532 0.0010640253
		 1.47974646 1.67545593 0.00095161994 1.4272356 1.95892215 0.0019049107 0.40506709 1.81334734 0.18054472
		 0.46376953 1.81479406 -0.19818 1.39337993 1.81804216 -0.30967411 1.50773883 1.81735694 0.0014219567
		 1.39345872 1.81596422 0.31254461 -4.1104842e-18 0.81271845 -0.49079341 0.53900892 0.81378514 -0.31242284
		 0.3893292 0.81080103 0.44240379 1.2554343e-17 0.80927163 0.54132074 2.8299867e-18 1.37244046 0.48229817
		 0.37809452 1.39679205 0.39670235 0.52364457 1.37652695 -0.25623548 0.67096984 0.81315601 -0.0018256374
		 0.62580895 1.37598705 5.969725e-05 1.3823128e-17 0.50414807 -0.49571913 0.54069 0.50519013 -0.30910951
		 0.67192447 0.50457329 -0.0028562769 0.38900661 0.50221431 0.44137329 -5.4290552e-20 0.50074196 0.52430594
		 1.3823305e-17 0.50250202 -0.0027782985 0.22408895 2.21961808 0.0029278593 0.21858813 2.21994114 -0.098871268
		 0.21356103 2.21924543 0.10472887 0.53561503 1.95575297 0.1249675 0.52290893 1.95612419 0.0020100777
		 0.51054728 1.95648575 -0.11761237 0.53339452 1.40345252 0.219128 0.58821726 0.81203419 0.25740972
		 0.5888074 0.50345761 0.25419107 -5.4308084e-20 0.50176036 0.21932034 0.29676005 1.68174434 -0.28339362
		 0.30805656 1.3762027 -0.35882059 0.30341017 0.81351227 -0.4489544 0.30619597 0.50488925 -0.43632656
		 2.764626e-17 0.50352478 -0.30904737 0.35888514 1.81552362 0.1936563 0.39583299 1.67073941 0.25454637
		 0.51703459 1.61822295 0.17618583 0.53570056 1.61316967 0.00086315972 0.52369559 1.61387563 -0.22156514
		 0.44353703 1.67622662 -0.23780085 2.1719513e-18 1.99043906 0.190459 -1.1190733e-17 1.99174404 -0.18189329
		 -2.7191869e-17 1.37887895 0.47969031 -1.2825472e-17 1.69669068 -0.32333955 -8.1544772e-18 1.34508383 -0.41945654
		 7.3186757e-18 2.21892524 0.0029539354 -1.0118611e-19 2.21953845 -0.18060446 6.3594125e-18 2.21829748 0.19080335
		 -8.847236e-18 1.37543869 -0.41537187 2.5627106e-18 1.83243668 0.29554787 -5.2713681e-18 1.8319236 0.2958039
		 0.86344719 1.6728251 0.21778667 0.7902326 1.81436706 0.23198365 0.85849118 1.9566381 0.15901279
		 0.85594422 1.95715463 0.001971348 0.84342945 1.95763397 -0.15306792 0.80012375 1.81596947 -0.23852111
		 0.86833012 1.67438698 -0.24525417 0.86555117 1.67355573 0.0010230463 0.9108364 1.81468642 0.24809028
		 0.90817124 1.816347 -0.25147995 -0.51294464 1.67191553 0.16545029 -1.38533735 1.67417932 0.29571429
		 -0.25893268 1.95496762 0.10384056 -1.34280539 1.95796573 0.2100807 -0.34301886 1.95590782 -0.09976878
		 -1.3427527 1.95935607 -0.20625122 -0.52370834 1.67321277 -0.21289755 -1.38526273 1.67614806 -0.29378909
		 -0.35158592 1.9555943 0.0020300017 -0.51317346 1.67246532 0.0010640253 -1.47974646 1.67545593 0.00095161994
		 -1.4272356 1.95892215 0.0019049107 -0.40506709 1.81334734 0.18054472 -0.46376953 1.81479406 -0.19818
		 -1.39337993 1.81804216 -0.30967411 -1.50773883 1.81735694 0.0014219567 -1.39345872 1.81596422 0.31254461
		 -0.53900892 0.81378514 -0.31242284 -0.3893292 0.81080103 0.44240379 -0.37809452 1.39679205 0.39670235
		 -0.52364457 1.37652695 -0.25623548 -0.67096984 0.81315601 -0.0018256374 -0.62580895 1.37598705 5.969725e-05
		 -0.54069 0.50519013 -0.30910951 -0.67192447 0.50457329 -0.0028562769 -0.38900661 0.50221431 0.44137329
		 -0.22408895 2.21961808 0.0029278593 -0.21858813 2.21994114 -0.098871268 -0.21356103 2.21924543 0.10472887
		 -0.53561503 1.95575297 0.1249675 -0.52290893 1.95612419 0.0020100777 -0.51054728 1.95648575 -0.11761237
		 -0.53339452 1.40345252 0.219128 -0.58821726 0.81203419 0.25740972 -0.5888074 0.50345761 0.25419107
		 -0.29676005 1.68174434 -0.28339362 -0.30805656 1.3762027 -0.35882059 -0.30341017 0.81351227 -0.4489544
		 -0.30619597 0.50488925 -0.43632656 -0.35888514 1.81552362 0.1936563 -0.39583299 1.67073941 0.25454637
		 -0.51703459 1.61822295 0.17618583 -0.53570056 1.61316967 0.00086315972 -0.52369559 1.61387563 -0.22156514
		 -0.44353703 1.67622662 -0.23780085 -0.86344719 1.6728251 0.21778667 -0.7902326 1.81436706 0.23198365
		 -0.85849118 1.9566381 0.15901279 -0.85594422 1.95715463 0.001971348 -0.84342945 1.95763397 -0.15306792
		 -0.80012375 1.81596947 -0.23852111 -0.86833012 1.67438698 -0.24525417 -0.86555117 1.67355573 0.0010230463
		 -0.9108364 1.81468642 0.24809028 -0.90817124 1.816347 -0.25147995 1.32894802 4.37142181 -0.064256519
		 1.2787993 4.4261055 -0.064256549 1.25372493 4.45344734 -1.1056223e-08 1.2787993 4.4261055 0.064256549
		 1.32894802 4.37142181 0.064256549 1.35402238 4.34407997 0 1.40870619 4.39422894 -0.12851304
		 1.3084085 4.50359583 -0.1285131 1.25825977 4.55827951 -2.2112445e-08 1.3084085 4.50359583 0.1285131
		 1.40870607 4.39422894 0.1285131 1.45885491 4.33954525 0 1.37909675 4.31673813 -0.19276965
		 1.22865057 4.48078918 -0.19276965 1.15342736 4.56281471 -3.3168675e-08 1.22865057 4.48078918 0.19276965
		 1.37909675 4.31673813 0.19276965 1.45431995 4.2347126 0 1.29933882 4.29393101 -0.12851304
		 1.19904113 4.40329838 -0.1285131 1.1488924 4.45798206 -2.2112445e-08 1.19904113 4.40329838 0.12851304
		 1.2993387 4.29393101 0.1285131 1.34948754 4.23924732 0 1.2540127 4.19300413 -0.064256519
		 1.20386398 4.24768782 -0.064256549 1.17878962 4.27502966 -1.1056223e-08 1.20386386 4.24768782 0.064256549
		 1.2540127 4.19300413 0.064256549 1.27908707 4.16566229 0 1.33377075 4.21581125 -0.12851304
		 1.23347318 4.32517862 -0.1285131 1.18332458 4.37986231 -2.2112445e-08;
	setAttr ".vt[332:434]" 1.23347318 4.32517862 0.1285131 1.33377075 4.21581125 0.1285131
		 1.38391948 4.16112757 0 1.30416155 4.13832045 -0.19276965 1.15371525 4.3023715 -0.19276965
		 1.078491926 4.38439703 -3.3168675e-08 1.15371513 4.30237198 0.19276965 1.30416155 4.13832092 0.19276965
		 1.37938464 4.056295395 0 1.22440338 4.1155138 -0.12851304 1.12410593 4.22488117 -0.1285131
		 1.073956966 4.27956486 -2.2112445e-08 1.12410593 4.22488117 0.12851304 1.22440338 4.1155138 0.1285131
		 1.27455211 4.060830116 0 1.18154764 4.012988091 -0.064256519 1.13139892 4.067671776 -0.064256549
		 1.10632443 4.095013618 -1.1056223e-08 1.13139892 4.067671776 0.064256549 1.18154752 4.012988091 0.064256549
		 1.20662189 3.98564625 0 1.26130569 4.035795212 -0.12851304 1.16100812 4.14516258 -0.1285131
		 1.11085939 4.19984627 -2.2112445e-08 1.16100812 4.14516258 0.1285131 1.26130569 4.035795212 0.1285131
		 1.31145442 3.98111129 0 1.23169637 3.95830441 -0.19276965 1.081250072 4.12235546 -0.19276965
		 1.0060269833 4.20438099 -3.3168675e-08 1.081249952 4.12235546 0.19276965 1.23169637 3.95830441 0.19276965
		 1.30691946 3.87627888 0 1.15193832 3.93549728 -0.12851304 1.051640749 4.044864655 -0.1285131
		 1.0014920235 4.09954834 -2.2112445e-08 1.051640749 4.044864655 0.12851304 1.1519382 3.93549728 0.1285131
		 1.20208704 3.8808136 0 0.11932686 0.54610449 0.13755175 0.11932686 0.54610449 -0.143104
		 0.11932686 0.0072156787 -0.143104 0.041917622 0.54610449 0 0.041917622 0.0072156787 0
		 0.11932686 0.33054876 0.13755174 0.041917622 0.33054876 0 0.11932686 0.33054876 -0.143104
		 0.11932686 0.0072156787 0.31350029 0.11932686 0.298949 0.27204451 0.041917622 0.298949 0.13449277
		 0.041917622 0.0072156787 0.17594856 0.11060342 0.16888231 0.33760336 0.016828954 0.16888231 0.18535081
		 0.016828954 0.16888231 -0.0094022425 0.11060342 0.16888231 -0.16780047 0.45080459 0.54610449 0.13755174
		 0.45080459 0.54610449 -0.143104 0.45080459 0.0072156787 -0.143104 0.52821386 0.54610449 -8.4423211e-18
		 0.52821386 0.0072156787 5.6282139e-19 0.45080459 0.33054876 0.13755174 0.52821386 0.33054876 -4.6292063e-18
		 0.45080459 0.33054876 -0.143104 0.45080459 0.0072156787 0.31350029 0.45080459 0.298949 0.27204451
		 0.52821386 0.298949 0.13449277 0.52821386 0.0072156787 0.17594856 0.45952803 0.16888231 0.33760336
		 0.55330265 0.16888231 0.18535081 0.55330265 0.16888231 -0.0094022425 0.45952803 0.16888231 -0.16780047
		 -0.11932683 0.54610449 0.13755175 -0.11932683 0.54610449 -0.143104 -0.11932683 0.0072156787 -0.143104
		 -0.041917622 0.54610449 -3.469447e-18 -0.041917622 0.0072156787 -2.4311703e-18 -0.11932683 0.33054876 0.13755174
		 -0.041917622 0.33054876 -8.9986472e-18 -0.11932683 0.33054876 -0.143104 -0.11932683 0.0072156787 0.31350029
		 -0.11932683 0.298949 0.27204451 -0.041917622 0.298949 0.13449277 -0.041917622 0.0072156787 0.17594856
		 -0.11060339 0.16888231 0.33760336 -0.016828895 0.16888231 0.18535081 -0.016828895 0.16888231 -0.0094022425
		 -0.11060339 0.16888231 -0.16780047 -0.45080459 0.54610449 0.13755174 -0.45080459 0.54610449 -0.143104
		 -0.45080459 0.0072156787 -0.143104 -0.52821386 0.54610449 -2.2320109e-17 -0.52821386 0.0072156787 -1.4123092e-17
		 -0.45080459 0.33054876 0.13755174 -0.52821386 0.33054876 -1.0038398e-17 -0.45080459 0.33054876 -0.143104
		 -0.45080459 0.0072156787 0.31350029 -0.45080459 0.298949 0.27204451 -0.52821386 0.298949 0.13449277
		 -0.52821386 0.0072156787 0.17594856 -0.45952797 0.16888231 0.33760336 -0.55330265 0.16888231 0.18535081
		 -0.55330265 0.16888231 -0.0094022425 -0.45952797 0.16888231 -0.16780047;
	setAttr -s 855 ".ed";
	setAttr ".ed[0:165]"  0 21 0 1 33 0 2 14 0 3 30 0 4 8 0 5 9 0 6 10 0 7 11 0
		 4 69 0 5 35 1 6 12 1 7 28 1 8 0 0 9 1 0 10 2 0 11 3 0 8 24 1 9 34 1 10 13 1 11 29 1
		 12 18 1 13 19 1 14 20 0 15 1 0 16 9 1 17 5 1 12 13 1 13 36 1 14 32 1 15 25 1 16 17 1
		 18 7 1 19 11 1 20 3 0 21 15 0 22 16 1 23 17 0 18 19 1 19 37 1 20 31 1 21 26 1 22 38 1
		 24 22 1 24 4 1 25 16 1 26 22 1 1 25 1 25 26 1 26 27 1 27 0 1 24 27 1 27 21 1 28 4 1
		 29 8 1 30 0 0 31 21 1 32 15 1 33 2 0 34 10 1 35 6 1 28 29 1 29 30 1 30 31 1 31 32 1
		 32 33 1 33 34 1 34 35 1 36 14 1 37 20 1 2 36 1 36 37 1 37 3 1 38 23 1 39 23 1 17 38 1
		 38 39 1 39 22 1 39 24 1 61 40 0 61 62 1 62 54 1 40 54 0 56 4 1 4 44 0 44 56 1 65 35 1
		 5 45 0 45 65 1 18 52 1 52 47 1 7 47 0 28 60 1 60 44 1 54 58 1 58 59 1 59 54 1 64 65 1
		 45 41 0 41 64 0 52 67 1 67 43 1 47 43 0 60 61 1 44 40 0 12 48 1 6 46 0 46 48 1 42 66 1
		 48 66 1 46 42 0 63 50 1 63 64 1 50 41 0 41 57 1 50 57 1 51 17 1 51 45 1 48 52 1 66 67 1
		 62 63 1 54 50 0 57 58 1 17 68 1 55 68 1 55 51 1 68 39 1 39 55 1 57 51 1 58 55 1 56 59 1
		 56 55 1 59 40 1 47 60 1 43 61 0 53 43 0 53 62 1 49 53 0 49 63 1 42 49 0 64 42 0 65 46 1
		 66 49 1 67 53 1 68 23 1 39 56 1 17 22 1 17 55 1 69 39 0 24 69 1 69 56 1 70 91 0 71 105 0
		 72 84 0 73 102 0 74 78 0 75 79 0 76 80 0 77 81 0 75 107 1 76 82 1 77 100 1 78 70 0
		 79 71 0 80 72 0 81 73 0 79 106 1;
	setAttr ".ed[166:331]" 80 83 1 81 101 1 82 88 1 83 89 1 84 90 0 85 71 0 86 79 1
		 87 75 1 82 83 1 83 84 1 84 104 1 85 86 1 86 87 1 88 77 1 89 81 1 90 73 0 91 85 0
		 92 86 1 93 87 1 88 89 1 89 90 1 90 103 1 91 92 0 92 93 0 92 94 0 93 95 1 94 95 0
		 74 96 1 96 95 1 78 97 0 96 97 0 97 94 1 91 98 0 98 94 0 70 99 0 97 99 0 99 98 0 100 74 1
		 101 78 1 102 70 0 103 91 1 104 85 1 105 72 0 106 80 1 107 76 1 100 101 1 101 102 1
		 102 103 1 103 104 1 104 105 1 105 106 1 106 107 1 129 108 0 129 130 1 130 122 1 108 122 0
		 124 95 0 96 125 0 125 124 1 133 107 1 75 113 0 113 133 1 88 120 1 120 115 1 77 115 0
		 100 128 1 128 112 1 74 112 0 126 124 0 125 127 0 127 126 0 132 133 1 113 109 0 109 132 0
		 120 121 1 121 111 0 115 111 0 128 129 1 112 108 0 82 116 1 76 114 0 114 116 1 116 117 1
		 114 110 0 110 117 0 131 118 1 131 132 1 118 109 0 119 113 1 118 119 1 119 87 1 116 120 1
		 117 121 0 130 131 1 122 118 0 123 119 1 122 123 0 123 93 0 123 124 0 112 125 0 122 126 0
		 108 127 0 115 128 1 111 129 0 121 130 1 117 131 1 132 110 0 133 114 1 134 142 0 136 143 0
		 138 144 0 140 145 0 134 146 0 135 150 0 136 138 0 137 139 0 138 147 0 139 149 0 140 134 0
		 141 135 0 142 135 0 143 137 0 144 139 0 145 141 0 145 148 1 144 143 1 143 151 1 146 136 0
		 147 140 0 148 144 1 149 141 0 150 137 0 151 142 1 146 147 1 147 148 1 148 149 1 149 150 1
		 150 151 1 151 146 1 168 169 1 169 160 1 160 153 0 153 168 0 162 161 1 161 155 0 155 157 0
		 162 157 0 163 166 1 166 167 1 167 159 0 163 159 0 158 163 0 159 153 0 152 160 0 158 152 0
		 167 168 1 152 164 0 164 165 1 165 158 0 165 166 1 154 161 0 156 162 0 154 156 0 169 164 1
		 164 154 0 156 165 0;
	setAttr ".ed[332:497]" 166 162 1 157 167 0 168 155 0 161 169 1 170 234 0 172 205 0
		 174 207 0 176 240 0 170 182 0 171 186 0 172 178 0 173 181 0 174 183 0 175 184 0 176 179 0
		 177 180 0 178 174 0 179 170 0 180 171 0 181 175 0 179 241 1 180 185 1 181 237 1 182 172 0
		 183 176 0 184 177 0 185 181 1 186 173 0 183 239 1 184 185 1 185 186 1 186 242 1 176 221 0
		 182 217 1 187 227 1 189 192 0 187 214 0 188 194 0 189 190 0 191 190 1 192 218 0 193 188 0
		 191 192 1 192 208 1 193 213 1 194 209 0 195 193 1 179 220 1 195 194 1 187 196 1 188 197 0
		 196 215 0 194 198 0 197 198 0 189 199 0 190 200 1 199 200 0 198 210 0 200 211 1 201 216 1
		 201 198 1 178 202 0 174 203 0 202 203 0 172 204 0 204 202 0 205 236 0 206 178 1 207 238 0
		 182 205 1 205 206 1 206 207 1 207 183 1 208 195 1 209 189 0 210 199 0 211 201 1 170 219 1
		 208 209 1 209 210 1 210 211 1 212 222 1 213 231 1 214 188 0 215 197 0 174 212 1 212 213 1
		 213 214 1 214 215 1 216 196 1 197 216 1 217 232 1 218 182 0 219 208 1 220 195 1 221 193 0
		 222 176 1 218 219 1 219 220 1 220 221 1 221 212 1 223 172 0 224 174 0 225 191 1 226 212 1
		 228 202 1 229 203 0 230 204 0 233 218 1 228 229 1 226 224 1 223 232 0 231 226 1 232 233 0
		 227 231 1 228 230 1 224 229 0 223 230 0 233 225 1 235 182 1 236 173 0 237 206 1 238 175 0
		 239 243 1 240 177 0 241 180 1 234 235 1 235 236 1 236 237 1 237 238 1 238 239 1 239 240 1
		 240 241 1 242 235 1 234 171 0 241 234 1 236 242 1 242 234 1 243 184 1 238 243 1 243 240 1
		 244 289 0 289 290 1 290 256 1 244 256 0 246 273 0 273 274 1 274 252 1 246 252 0 257 294 1
		 294 295 1 250 295 0 257 250 0 253 296 1 296 289 1 253 244 0 254 245 0 254 259 1 259 260 1
		 245 260 0 295 296 1 250 253 0 258 259 1 251 254 0 258 251 0 274 275 1;
	setAttr ".ed[498:663]" 248 275 0 252 248 0 275 257 1 248 257 0 259 255 1 249 258 0
		 255 249 0 247 255 0 260 247 0 256 273 1 256 246 0 248 279 1 226 279 1 224 248 0 256 283 1
		 223 246 0 283 232 1 279 280 1 280 231 1 253 286 1 286 287 1 250 287 0 284 256 0 233 284 1
		 191 263 1 262 190 0 262 263 0 266 265 1 261 265 0 264 261 0 266 264 1 280 281 1 187 281 0
		 228 270 1 230 272 0 272 270 0 284 285 1 244 285 1 276 277 1 265 277 0 276 266 1 281 282 1
		 196 282 0 265 268 0 267 268 0 261 267 0 269 200 0 262 269 0 277 278 1 268 278 0 201 268 1
		 278 211 1 267 216 1 282 267 0 285 286 1 248 271 0 270 271 0 252 270 0 229 271 0 246 272 0
		 290 291 1 273 291 0 291 292 1 292 274 1 292 293 1 275 293 0 293 294 1 277 262 0 263 276 1
		 278 269 0 288 250 1 279 288 1 287 279 1 281 261 0 264 280 1 263 284 0 285 276 1 286 266 1
		 287 264 0 297 289 1 289 245 0 260 297 1 291 297 1 291 247 0 255 292 1 293 249 0 293 298 1
		 294 298 1 298 295 1 295 251 0 296 254 1 297 290 1 298 258 1 299 300 0 300 301 0 301 302 0
		 302 303 0 303 304 0 304 299 0 305 306 0 306 307 0 307 308 0 308 309 0 309 310 0 310 305 0
		 311 312 0 312 313 0 313 314 0 314 315 0 315 316 0 316 311 0 317 318 0 318 319 0 319 320 0
		 320 321 0 321 322 0 322 317 0 299 305 0 300 306 0 301 307 0 302 308 0 303 309 0 304 310 0
		 305 311 0 306 312 0 307 313 0 308 314 0 309 315 0 310 316 0 311 317 0 312 318 0 313 319 0
		 314 320 0 315 321 0 316 322 0 317 299 0 318 300 0 319 301 0 320 302 0 321 303 0 322 304 0
		 323 324 0 324 325 0 325 326 0 326 327 0 327 328 0 328 323 0 329 330 0 330 331 0 331 332 0
		 332 333 0 333 334 0 334 329 0 335 336 0 336 337 0 337 338 0 338 339 0 339 340 0 340 335 0
		 341 342 0 342 343 0 343 344 0 344 345 0 345 346 0 346 341 0 323 329 0;
	setAttr ".ed[664:829]" 324 330 0 325 331 0 326 332 0 327 333 0 328 334 0 329 335 0
		 330 336 0 331 337 0 332 338 0 333 339 0 334 340 0 335 341 0 336 342 0 337 343 0 338 344 0
		 339 345 0 340 346 0 341 323 0 342 324 0 343 325 0 344 326 0 345 327 0 346 328 0 347 348 0
		 348 349 0 349 350 0 350 351 0 351 352 0 352 347 0 353 354 0 354 355 0 355 356 0 356 357 0
		 357 358 0 358 353 0 359 360 0 360 361 0 361 362 0 362 363 0 363 364 0 364 359 0 365 366 0
		 366 367 0 367 368 0 368 369 0 369 370 0 370 365 0 347 353 0 348 354 0 349 355 0 350 356 0
		 351 357 0 352 358 0 353 359 0 354 360 0 355 361 0 356 362 0 357 363 0 358 364 0 359 365 0
		 360 366 0 361 367 0 362 368 0 363 369 0 364 370 0 365 347 0 366 348 0 367 349 0 368 350 0
		 369 351 0 370 352 0 371 374 0 372 378 0 373 375 0 374 372 0 374 377 1 376 371 0 377 385 0
		 378 386 0 376 377 0 377 378 1 376 380 0 379 383 0 377 381 0 380 381 0 375 382 0 381 384 0
		 382 379 0 383 380 0 384 382 0 385 375 0 386 373 0 383 384 1 384 385 1 385 386 1 395 379 0
		 383 399 1 395 399 0 387 371 0 374 390 1 387 390 0 402 386 1 389 373 0 402 389 0 391 375 0
		 399 400 1 400 398 0 398 395 0 389 391 0 401 391 0 401 402 1 388 372 0 390 388 0 390 393 1
		 392 393 0 392 387 0 393 394 1 388 394 0 394 378 1 376 392 0 392 396 0 380 396 0 393 397 0
		 396 397 0 400 401 1 391 398 0 399 396 0 397 400 0 393 401 0 394 402 0 398 382 1 411 415 0
		 427 411 0 427 431 0 415 431 1 403 406 0 419 403 0 419 422 0 406 422 1 418 405 0 434 418 1
		 434 421 0 421 405 0 415 416 1 416 414 0 414 411 0 405 407 0 417 407 0 417 418 1 406 404 0
		 422 420 0 420 404 0 406 409 1 408 409 0 408 403 0 409 410 1 404 410 0 420 426 0 426 410 1
		 408 424 0 424 419 0 409 413 0 412 413 0 408 412 0 416 417 1 407 414 0;
	setAttr ".ed[830:854]" 430 414 1 423 407 0 423 430 0 415 412 0 431 428 0 412 428 0
		 413 416 0 409 417 0 410 418 0 426 434 0 430 427 0 432 430 0 431 432 1 433 434 1 433 423 0
		 421 423 0 424 425 0 422 425 1 425 426 1 424 428 0 428 429 0 425 429 0 432 433 1 429 432 0
		 425 433 0;
	setAttr -s 434 -ch 1710 ".fc[0:433]" -type "polyFaces" 
		f 4 -55 62 55 -1
		mu 0 4 0 80 78 21
		f 3 43 4 16
		mu 0 3 24 4 8
		f 4 66 -10 5 17
		mu 0 4 72 71 5 9
		f 4 -32 37 32 -8
		mu 0 4 7 18 19 11
		f 4 -53 60 53 -5
		mu 0 4 4 28 29 8
		f 3 40 48 51
		mu 0 3 21 26 27
		f 4 65 -18 13 1
		mu 0 4 74 72 9 1
		f 4 -33 38 71 -16
		mu 0 4 11 19 37 3
		f 4 -54 61 54 -13
		mu 0 4 8 29 80 0
		f 4 -27 -11 6 18
		mu 0 4 13 12 6 10
		f 4 69 -28 -19 14
		mu 0 4 2 36 13 10
		f 4 -57 64 -2 -24
		mu 0 4 15 79 74 1
		f 3 46 -30 23
		mu 0 3 1 25 15
		f 4 -26 -31 24 -6
		mu 0 4 5 17 16 9
		f 4 -38 -21 26 21
		mu 0 4 19 18 12 13
		f 4 70 -39 -22 27
		mu 0 4 36 37 19 13
		f 4 -56 63 56 -35
		mu 0 4 21 78 79 15
		f 4 29 47 -41 34
		mu 0 4 15 25 26 21
		f 3 30 145 35
		mu 0 3 16 17 22
		f 3 41 75 76
		mu 0 3 22 38 39
		f 4 -25 -45 -47 -14
		mu 0 4 9 16 25 1
		f 4 -48 44 -36 -46
		mu 0 4 26 25 16 22
		f 4 50 -49 45 -43
		mu 0 4 24 27 26 22
		f 4 -50 -51 -17 12
		mu 0 4 0 27 24 8
		f 3 -52 49 0
		mu 0 3 21 27 0
		f 4 -61 -12 7 19
		mu 0 4 29 28 7 11
		f 4 -62 -20 15 3
		mu 0 4 30 29 11 3
		f 4 -63 -4 -34 39
		mu 0 4 31 30 3 20
		f 4 -64 -40 -23 28
		mu 0 4 32 31 20 14
		f 4 -65 -29 -3 -58
		mu 0 4 33 32 14 2
		f 4 -59 -66 57 -15
		mu 0 4 10 34 33 2
		f 4 -60 -67 58 -7
		mu 0 4 6 35 34 10
		f 3 -68 -70 2
		mu 0 3 14 36 2
		f 4 -69 -71 67 22
		mu 0 4 20 37 36 14
		f 3 -72 68 33
		mu 0 3 3 37 20
		f 3 -75 -37 -73
		mu 0 3 38 17 23
		f 3 -76 72 -74
		mu 0 3 39 38 23
		f 3 -77 77 42
		mu 0 3 22 39 24
		f 3 148 -9 -44
		mu 0 3 24 69 4
		f 4 81 -81 -80 78
		mu 0 4 40 43 75 77
		f 3 -85 -84 -83
		mu 0 3 44 45 4
		f 4 -88 -87 9 -86
		mu 0 4 70 47 5 71
		f 4 90 -90 -89 31
		mu 0 4 7 49 48 18
		f 4 83 -93 -92 52
		mu 0 4 4 45 50 28
		f 3 -96 -95 -94
		mu 0 3 43 52 51
		f 4 -99 -98 87 -97
		mu 0 4 73 54 47 70
		f 4 101 -101 -100 89
		mu 0 4 49 56 55 48
		f 4 103 -79 -103 92
		mu 0 4 45 40 77 50
		f 4 -107 -106 10 104
		mu 0 4 57 58 6 12
		f 4 -110 106 108 -108
		mu 0 4 59 58 57 60
		f 4 112 98 -112 110
		mu 0 4 61 54 73 76
		f 3 -113 114 -114
		mu 0 3 54 61 63
		f 4 86 -117 115 25
		mu 0 4 5 47 64 17
		f 4 -118 -105 20 88
		mu 0 4 48 57 12 18
		f 4 -109 117 99 -119
		mu 0 4 60 57 48 55
		f 4 120 -111 -120 80
		mu 0 4 43 61 76 75
		f 4 -121 93 -122 -115
		mu 0 4 61 43 51 63
		f 3 146 123 -123
		mu 0 3 17 66 65
		f 3 -127 -126 -124
		mu 0 3 66 39 65
		f 4 97 113 127 116
		mu 0 4 47 54 63 64
		f 4 128 124 -128 121
		mu 0 4 51 66 64 63
		f 4 130 -129 94 -130
		mu 0 4 44 66 51 52
		f 4 -104 84 129 131
		mu 0 4 40 45 44 52
		f 3 -82 -132 95
		mu 0 3 43 40 52
		f 4 -133 -91 11 91
		mu 0 4 50 49 7 28
		f 4 -134 -102 132 102
		mu 0 4 41 56 49 50
		f 4 -136 134 133 79
		mu 0 4 42 67 56 41
		f 4 -138 136 135 119
		mu 0 4 62 68 67 42
		f 4 139 138 137 111
		mu 0 4 53 59 68 62
		f 4 109 -140 96 140
		mu 0 4 58 59 53 46
		f 4 105 -141 85 59
		mu 0 4 6 58 46 35
		f 3 -139 107 141
		mu 0 3 68 59 60
		f 4 -137 -142 118 142
		mu 0 4 67 68 60 55
		f 3 -135 -143 100
		mu 0 3 56 67 55
		f 3 143 36 122
		mu 0 3 65 23 17
		f 3 73 -144 125
		mu 0 3 39 23 65
		f 3 -131 -145 126
		mu 0 3 66 44 39
		f 3 82 8 149
		mu 0 3 44 4 69
		f 3 -146 74 -42
		mu 0 3 22 17 38
		f 3 -125 -147 -116
		mu 0 3 64 66 17
		f 3 -78 -148 -149
		mu 0 3 24 39 69
		f 3 -150 147 144
		mu 0 3 44 69 39
		f 4 -206 213 206 -151
		mu 0 4 81 82 83 84
		f 4 192 -195 196 197
		mu 0 4 85 86 87 88
		f 4 217 -159 155 165
		mu 0 4 89 90 91 92
		f 4 -180 185 180 -158
		mu 0 4 93 94 95 96
		f 4 -204 211 204 -155
		mu 0 4 97 98 99 100
		f 4 199 -198 201 202
		mu 0 4 101 85 88 102
		f 4 216 -166 162 151
		mu 0 4 103 89 92 104
		f 4 -181 186 181 -165
		mu 0 4 96 95 105 106
		f 4 -205 212 205 -162
		mu 0 4 100 99 107 108
		f 4 -175 -160 156 166
		mu 0 4 109 110 111 112
		f 4 -176 -167 163 152
		mu 0 4 113 109 112 114
		f 4 -208 215 -152 -172
		mu 0 4 115 116 103 104
		f 4 -173 -178 171 -163
		mu 0 4 92 117 115 104
		f 4 -174 -179 172 -156
		mu 0 4 91 118 117 92
		f 4 -186 -169 174 169
		mu 0 4 95 94 110 109
		f 4 -187 -170 175 170
		mu 0 4 105 95 109 113
		f 4 -207 214 207 -183
		mu 0 4 84 83 116 115
		f 4 177 -184 -189 182
		mu 0 4 115 117 119 84
		f 4 178 -185 -190 183
		mu 0 4 117 118 120 119
		f 4 189 191 -193 -191
		mu 0 4 119 120 86 85
		f 4 154 195 -197 -194
		mu 0 4 97 100 88 87
		f 4 188 190 -200 -199
		mu 0 4 121 119 85 101
		f 4 161 200 -202 -196
		mu 0 4 100 108 102 88
		f 4 150 198 -203 -201
		mu 0 4 122 121 101 102
		f 4 -212 -161 157 167
		mu 0 4 99 98 123 124
		f 4 -213 -168 164 153
		mu 0 4 107 99 124 125
		f 4 -214 -154 -182 187
		mu 0 4 83 82 106 105
		f 4 -215 -188 -171 176
		mu 0 4 116 83 105 113
		f 4 -216 -177 -153 -209
		mu 0 4 103 116 113 114
		f 4 -210 -217 208 -164
		mu 0 4 112 89 103 114
		f 4 -211 -218 209 -157
		mu 0 4 111 90 89 112
		f 4 221 -221 -220 218
		mu 0 4 126 127 128 129
		f 4 -225 -224 194 -223
		mu 0 4 130 131 87 86
		f 4 -228 -227 158 -226
		mu 0 4 132 133 91 90
		f 4 230 -230 -229 179
		mu 0 4 93 134 135 94
		f 4 233 -233 -232 203
		mu 0 4 97 136 137 98
		f 4 -237 -236 224 -235
		mu 0 4 138 139 131 130
		f 4 -240 -239 227 -238
		mu 0 4 140 141 133 132
		f 4 242 -242 -241 229
		mu 0 4 134 142 143 135
		f 4 244 -219 -244 232
		mu 0 4 136 144 145 137
		f 4 -248 -247 159 245
		mu 0 4 146 147 111 110
		f 4 -251 -250 247 248
		mu 0 4 148 149 147 146
		f 4 253 239 -253 251
		mu 0 4 150 141 140 151
		f 4 238 -254 255 254
		mu 0 4 133 141 150 152
		f 4 226 -255 256 173
		mu 0 4 91 133 152 118
		f 4 -258 -246 168 228
		mu 0 4 135 146 110 94
		f 4 -259 -249 257 240
		mu 0 4 143 148 146 135
		f 4 260 -252 -260 220
		mu 0 4 127 150 151 128
		f 4 -261 262 261 -256
		mu 0 4 150 127 153 152
		f 4 -262 263 184 -257
		mu 0 4 152 153 120 118
		f 4 264 222 -192 -264
		mu 0 4 153 130 86 120
		f 4 193 223 -266 -234
		mu 0 4 97 87 131 136
		f 4 266 234 -265 -263
		mu 0 4 154 138 130 153
		f 4 265 235 -268 -245
		mu 0 4 136 131 139 144
		f 4 267 236 -267 -222
		mu 0 4 155 139 138 154
		f 4 -269 -231 160 231
		mu 0 4 137 156 123 98
		f 4 -270 -243 268 243
		mu 0 4 145 157 156 137
		f 4 -271 241 269 219
		mu 0 4 128 143 142 129
		f 4 -272 258 270 259
		mu 0 4 151 148 143 128
		f 4 272 250 271 252
		mu 0 4 140 149 148 151
		f 4 249 -273 237 273
		mu 0 4 147 149 140 132
		f 4 246 -274 225 210
		mu 0 4 111 147 132 90
		f 4 303 298 286 279
		mu 0 4 158 159 160 161
		f 4 291 287 281 -289
		mu 0 4 162 163 164 165
		f 4 290 301 296 -290
		mu 0 4 166 167 168 169
		f 6 277 289 285 -287 -275 -285
		mu 0 6 170 166 169 171 172 173
		f 4 -286 -297 302 -280
		mu 0 4 161 174 175 158
		f 4 284 278 299 294
		mu 0 4 176 177 178 179
		f 4 300 -291 -278 -295
		mu 0 4 180 167 166 170
		f 4 275 -292 -277 -281
		mu 0 4 181 163 162 182
		f 4 274 -299 304 -279
		mu 0 4 177 160 159 178
		f 4 -300 293 280 282
		mu 0 4 179 178 181 183
		f 4 276 -296 -301 -283
		mu 0 4 182 162 167 180
		f 4 -302 295 288 283
		mu 0 4 168 167 162 165
		f 4 -303 -284 -282 -298
		mu 0 4 158 175 184 164
		f 4 292 -304 297 -288
		mu 0 4 163 159 158 164
		f 4 -305 -293 -276 -294
		mu 0 4 178 159 163 181
		f 4 -309 -308 -307 -306
		mu 0 4 185 186 187 188
		f 4 312 -312 -311 -310
		mu 0 4 189 190 191 192
		f 4 316 -316 -315 -314
		mu 0 4 193 194 195 196
		f 6 320 319 307 -319 -317 -318
		mu 0 6 197 198 199 200 194 193
		f 4 308 -322 315 318
		mu 0 4 186 185 201 202
		f 4 -325 -324 -323 -321
		mu 0 4 203 204 205 206
		f 4 324 317 313 -326
		mu 0 4 207 197 193 196
		f 4 328 327 309 -327
		mu 0 4 208 209 189 192
		f 4 322 -330 306 -320
		mu 0 4 206 205 188 187
		f 4 -332 -329 -331 323
		mu 0 4 204 210 208 205
		f 4 331 325 332 -328
		mu 0 4 209 207 196 189
		f 4 -334 -313 -333 314
		mu 0 4 195 190 189 196
		f 4 334 311 333 321
		mu 0 4 185 191 211 201
		f 4 310 -335 305 -336
		mu 0 4 192 191 185 188
		f 4 330 326 335 329
		mu 0 4 205 208 192 188
		f 4 336 458 451 -341
		mu 0 4 212 213 214 215
		f 4 337 402 399 -343
		mu 0 4 216 217 218 219
		f 4 360 463 -340 -357
		mu 0 4 220 221 222 223
		f 4 352 467 -337 -350
		mu 0 4 224 225 226 227
		f 4 -351 353 362 -342
		mu 0 4 228 229 230 231
		f 4 339 464 -353 -347
		mu 0 4 232 233 225 224
		f 4 361 -354 -348 -358
		mu 0 4 234 230 229 235
		f 4 -400 403 -339 -349
		mu 0 4 219 218 236 237
		f 3 338 404 -345
		mu 0 3 237 236 238
		f 4 -359 -362 -346 -352
		mu 0 4 239 230 234 240
		f 4 -363 358 -344 -360
		mu 0 4 231 230 239 241
		f 3 401 -338 -356
		mu 0 3 242 217 216
		f 4 417 -437 442 434
		mu 0 4 237 243 244 245
		f 5 -366 355 -434 443 -424
		mu 0 5 246 242 216 247 248
		f 4 418 414 444 436
		mu 0 4 243 249 250 244
		f 4 379 431 -365 346
		mu 0 4 251 252 253 254
		f 5 424 365 423 445 440
		mu 0 5 255 242 246 248 256
		f 4 -375 371 -371 367
		mu 0 4 257 258 259 260
		f 4 380 -370 -374 -379
		mu 0 4 261 262 263 264
		f 5 419 -369 366 446 -415
		mu 0 5 249 265 266 267 250
		f 4 -438 447 439 397
		mu 0 4 268 269 270 271
		f 4 340 -425 429 -410
		mu 0 4 272 242 255 273
		f 4 410 -378 -381 -406
		mu 0 4 274 275 262 261
		f 4 368 420 -384 -382
		mu 0 4 266 265 276 277
		f 4 369 384 -386 -383
		mu 0 4 263 262 278 279
		f 4 370 387 -389 -387
		mu 0 4 260 259 280 281
		f 4 377 411 -390 -385
		mu 0 4 262 275 282 278
		f 4 392 389 412 408
		mu 0 4 283 284 285 286
		f 4 422 421 383 416
		mu 0 4 287 288 289 290
		f 4 409 430 -380 349
		mu 0 4 272 273 252 251
		f 4 348 394 -396 -394
		mu 0 4 291 292 293 268
		f 4 -435 448 438 -395
		mu 0 4 292 294 295 293
		f 4 342 393 -398 -397
		mu 0 4 216 291 268 271
		f 4 -452 459 -399 -402
		mu 0 4 215 214 296 297
		f 4 -403 398 460 453
		mu 0 4 298 297 296 299
		f 4 -404 -454 461 -401
		mu 0 4 300 298 299 301
		f 4 -405 400 462 -361
		mu 0 4 220 300 301 221
		f 4 -407 -411 -376 -368
		mu 0 4 260 275 274 257
		f 4 -412 406 386 -408
		mu 0 4 282 275 260 281
		f 4 -413 407 388 390
		mu 0 4 286 285 281 280
		f 5 344 356 -429 -414 -418
		mu 0 5 237 238 254 302 243
		f 4 428 364 432 413
		mu 0 4 302 254 253 243
		f 4 -416 -420 -377 373
		mu 0 4 263 265 249 264
		f 4 -421 415 382 -417
		mu 0 4 276 265 263 279
		f 4 385 -393 391 -423
		mu 0 4 287 284 283 288
		f 4 -430 -373 375 -426
		mu 0 4 273 255 257 274
		f 4 -431 425 405 -427
		mu 0 4 252 273 274 261
		f 4 -432 426 378 -428
		mu 0 4 253 252 261 264
		f 4 -433 427 376 -419
		mu 0 4 243 253 264 249
		f 5 -441 450 435 374 372
		mu 0 5 255 256 303 258 257
		f 4 -442 437 395 -439
		mu 0 4 295 269 268 293
		f 4 -450 433 396 -440
		mu 0 4 270 247 216 271
		f 4 469 466 341 363
		mu 0 4 304 213 228 231
		f 4 468 -364 359 -453
		mu 0 4 296 304 231 241
		f 4 -461 452 343 354
		mu 0 4 299 296 241 239
		f 4 -462 -355 351 -455
		mu 0 4 301 299 239 240
		f 3 -463 471 -456
		mu 0 3 221 301 305
		f 3 -464 455 472
		mu 0 3 222 221 305
		f 4 -465 456 347 -458
		mu 0 4 225 233 235 229
		f 4 -468 457 350 -467
		mu 0 4 226 225 229 228
		f 3 -460 -466 -469
		mu 0 3 296 214 304
		f 3 -459 -470 465
		mu 0 3 214 213 304
		f 4 -472 454 345 -471
		mu 0 4 305 301 240 234
		f 4 -473 470 357 -457
		mu 0 4 222 305 234 235
		f 4 476 -476 -475 -474
		mu 0 4 306 307 308 309
		f 4 480 -480 -479 -478
		mu 0 4 310 311 312 313
		f 4 484 483 -483 -482
		mu 0 4 314 315 316 317
		f 4 487 473 -487 -486
		mu 0 4 318 319 320 321
		f 4 491 -491 -490 488
		mu 0 4 322 323 324 325
		f 4 493 485 -493 -484
		mu 0 4 326 318 321 327
		f 4 496 495 489 -495
		mu 0 4 328 329 325 324
		f 4 499 498 -498 479
		mu 0 4 311 330 331 312
		f 3 501 -501 -499
		mu 0 3 330 332 331
		f 4 504 503 494 502
		mu 0 4 333 334 328 324
		f 4 506 505 -503 490
		mu 0 4 323 335 333 324
		f 3 508 477 -508
		mu 0 3 336 310 313
		f 4 -512 -443 510 -510
		mu 0 4 330 337 338 339
		f 5 514 -444 513 -509 512
		mu 0 5 340 341 342 310 336
		f 4 -511 -445 -517 -516
		mu 0 4 339 338 343 344
		f 4 -494 519 -519 -518
		mu 0 4 345 346 347 348
		f 5 -522 -446 -515 -513 -521
		mu 0 5 349 350 341 340 336
		f 4 -525 523 -372 522
		mu 0 4 351 352 353 354
		f 4 528 527 526 -526
		mu 0 4 355 356 357 358
		f 5 516 -447 -367 530 -530
		mu 0 5 344 343 359 360 361
		f 4 -534 -533 -448 531
		mu 0 4 362 363 364 365
		f 4 535 -535 520 -477
		mu 0 4 366 367 349 336
		f 4 538 525 537 -537
		mu 0 4 368 355 358 369
		f 4 381 540 -540 -531
		mu 0 4 360 370 371 361
		f 4 543 542 -542 -527
		mu 0 4 357 372 373 358
		f 4 545 544 -388 -524
		mu 0 4 352 374 375 353
		f 4 541 547 -547 -538
		mu 0 4 358 373 376 369
		f 4 -409 -550 -548 -549
		mu 0 4 377 378 379 380
		f 4 -552 -541 -422 -551
		mu 0 4 381 382 383 384
		f 4 -488 517 -553 -536
		mu 0 4 366 345 348 367
		f 4 555 554 -554 -500
		mu 0 4 385 362 386 387
		f 4 553 -557 -449 511
		mu 0 4 387 386 388 389
		f 4 557 533 -556 -481
		mu 0 4 310 363 362 385
		f 4 507 559 -559 475
		mu 0 4 307 390 391 308
		f 4 -562 -561 -560 478
		mu 0 4 392 393 391 390
		f 4 563 -563 561 497
		mu 0 4 394 395 393 392
		f 4 481 -565 -564 500
		mu 0 4 314 317 395 394
		f 4 524 566 536 565
		mu 0 4 352 351 368 369
		f 4 567 -546 -566 546
		mu 0 4 376 374 352 369
		f 4 -391 -545 -568 549
		mu 0 4 378 375 374 379
		f 5 509 569 568 -485 -502
		mu 0 5 330 339 396 346 332
		f 4 -570 -571 -520 -569
		mu 0 4 396 339 347 346
		f 4 -528 572 529 571
		mu 0 4 357 356 344 361
		f 4 551 -544 -572 539
		mu 0 4 371 372 357 361
		f 4 550 -392 548 -543
		mu 0 4 381 384 377 380
		f 4 574 -567 573 534
		mu 0 4 367 368 351 349
		f 4 575 -539 -575 552
		mu 0 4 348 355 368 367
		f 4 576 -529 -576 518
		mu 0 4 347 356 355 348
		f 4 515 -573 -577 570
		mu 0 4 339 344 356 347
		f 5 -574 -523 -436 -451 521
		mu 0 5 349 351 354 397 350
		f 4 556 -555 -532 441
		mu 0 4 388 386 362 365
		f 4 532 -558 -514 449
		mu 0 4 364 363 310 342
		f 4 -580 -492 -579 -578
		mu 0 4 398 323 322 309
		f 4 581 -507 579 -581
		mu 0 4 391 335 323 398
		f 4 -583 -506 -582 560
		mu 0 4 393 333 335 391
		f 4 583 -505 582 562
		mu 0 4 395 334 333 393
		f 3 585 -585 564
		mu 0 3 317 399 395
		f 3 -587 -586 482
		mu 0 3 316 399 317
		f 4 588 -496 -588 492
		mu 0 4 321 325 329 327
		f 4 578 -489 -589 486
		mu 0 4 320 322 325 321
		f 3 580 589 558
		mu 0 3 391 398 308
		f 3 -590 577 474
		mu 0 3 308 398 309
		f 4 590 -504 -584 584
		mu 0 4 399 328 334 395
		f 4 587 -497 -591 586
		mu 0 4 316 329 328 399
		f 4 -592 615 597 -617
		mu 0 4 400 401 402 403
		f 4 -593 616 598 -618
		mu 0 4 404 400 403 405
		f 4 -594 617 599 -619
		mu 0 4 406 407 408 409
		f 4 -595 618 600 -620
		mu 0 4 410 406 409 411
		f 4 -596 619 601 -621
		mu 0 4 412 410 411 413
		f 4 -597 620 602 -616
		mu 0 4 401 412 413 402
		f 4 -598 621 603 -623
		mu 0 4 403 402 414 415
		f 4 -599 622 604 -624
		mu 0 4 405 403 415 416
		f 4 -600 623 605 -625
		mu 0 4 409 408 417 418
		f 4 -601 624 606 -626
		mu 0 4 411 409 418 419
		f 4 -602 625 607 -627
		mu 0 4 413 411 419 420
		f 4 -603 626 608 -622
		mu 0 4 402 413 420 414
		f 4 -604 627 609 -629
		mu 0 4 415 414 421 422
		f 4 -605 628 610 -630
		mu 0 4 416 415 422 423
		f 4 -606 629 611 -631
		mu 0 4 418 417 424 425
		f 4 -607 630 612 -632
		mu 0 4 419 418 425 426
		f 4 -608 631 613 -633
		mu 0 4 420 419 426 427
		f 4 -609 632 614 -628
		mu 0 4 414 420 427 421
		f 4 -610 633 591 -635
		mu 0 4 422 421 428 429
		f 4 -611 634 592 -636
		mu 0 4 423 422 429 430
		f 4 -612 635 593 -637
		mu 0 4 425 424 431 432
		f 4 -613 636 594 -638
		mu 0 4 426 425 432 433
		f 4 -614 637 595 -639
		mu 0 4 427 426 433 434
		f 4 -615 638 596 -634
		mu 0 4 421 427 434 428
		f 4 -640 663 645 -665
		mu 0 4 435 436 437 438
		f 4 -641 664 646 -666
		mu 0 4 439 435 438 440
		f 4 -642 665 647 -667
		mu 0 4 441 442 443 444
		f 4 -643 666 648 -668
		mu 0 4 445 441 444 446
		f 4 -644 667 649 -669
		mu 0 4 447 445 446 448
		f 4 -645 668 650 -664
		mu 0 4 436 447 448 437
		f 4 -646 669 651 -671
		mu 0 4 438 437 449 450
		f 4 -647 670 652 -672
		mu 0 4 440 438 450 451
		f 4 -648 671 653 -673
		mu 0 4 444 443 452 453
		f 4 -649 672 654 -674
		mu 0 4 446 444 453 454
		f 4 -650 673 655 -675
		mu 0 4 448 446 454 455
		f 4 -651 674 656 -670
		mu 0 4 437 448 455 449
		f 4 -652 675 657 -677
		mu 0 4 450 449 456 457
		f 4 -653 676 658 -678
		mu 0 4 451 450 457 458
		f 4 -654 677 659 -679
		mu 0 4 453 452 459 460
		f 4 -655 678 660 -680
		mu 0 4 454 453 460 461
		f 4 -656 679 661 -681
		mu 0 4 455 454 461 462
		f 4 -657 680 662 -676
		mu 0 4 449 455 462 456
		f 4 -658 681 639 -683
		mu 0 4 457 456 463 464
		f 4 -659 682 640 -684
		mu 0 4 458 457 464 465
		f 4 -660 683 641 -685
		mu 0 4 460 459 466 467
		f 4 -661 684 642 -686
		mu 0 4 461 460 467 468
		f 4 -662 685 643 -687
		mu 0 4 462 461 468 469
		f 4 -663 686 644 -682
		mu 0 4 456 462 469 463
		f 4 -688 711 693 -713
		mu 0 4 470 471 472 473
		f 4 -689 712 694 -714
		mu 0 4 474 470 473 475
		f 4 -690 713 695 -715
		mu 0 4 476 477 478 479
		f 4 -691 714 696 -716
		mu 0 4 480 476 479 481
		f 4 -692 715 697 -717
		mu 0 4 482 480 481 483
		f 4 -693 716 698 -712
		mu 0 4 471 482 483 472
		f 4 -694 717 699 -719
		mu 0 4 473 472 484 485
		f 4 -695 718 700 -720
		mu 0 4 475 473 485 486
		f 4 -696 719 701 -721
		mu 0 4 479 478 487 488
		f 4 -697 720 702 -722
		mu 0 4 481 479 488 489
		f 4 -698 721 703 -723
		mu 0 4 483 481 489 490
		f 4 -699 722 704 -718
		mu 0 4 472 483 490 484
		f 4 -700 723 705 -725
		mu 0 4 485 484 491 492
		f 4 -701 724 706 -726
		mu 0 4 486 485 492 493
		f 4 -702 725 707 -727
		mu 0 4 488 487 494 495
		f 4 -703 726 708 -728
		mu 0 4 489 488 495 496
		f 4 -704 727 709 -729
		mu 0 4 490 489 496 497
		f 4 -705 728 710 -724
		mu 0 4 484 490 497 491
		f 4 -706 729 687 -731
		mu 0 4 492 491 498 499
		f 4 -707 730 688 -732
		mu 0 4 493 492 499 500
		f 4 -708 731 689 -733
		mu 0 4 495 494 501 502
		f 4 -709 732 690 -734
		mu 0 4 496 495 502 503
		f 4 -710 733 691 -735
		mu 0 4 497 496 503 504
		f 4 -711 734 692 -730
		mu 0 4 491 497 504 498
		f 4 -747 -760 761 -761
		mu 0 4 505 506 507 508
		f 4 -736 -763 764 -764
		mu 0 4 509 510 511 512
		f 4 -756 -766 767 766
		mu 0 4 513 514 515 516
		f 4 756 753 751 746
		mu 0 4 505 517 518 506
		f 4 737 -755 758 755
		mu 0 4 513 519 520 514
		f 4 -739 763 776 775
		mu 0 4 521 509 512 522
		f 4 739 -744 740 735
		mu 0 4 509 523 524 510
		f 4 -745 -740 738 736
		mu 0 4 525 523 509 521
		f 4 -737 -776 781 782
		mu 0 4 526 521 522 527
		f 4 -741 783 779 762
		mu 0 4 510 524 528 511
		f 4 743 747 -749 -746
		mu 0 4 529 530 531 532
		f 4 757 754 749 -754
		mu 0 4 517 520 519 518
		f 4 794 -750 -769 789
		mu 0 4 533 534 535 536
		f 4 -753 760 790 -786
		mu 0 4 532 505 508 537
		f 4 748 750 -757 752
		mu 0 4 532 531 517 505
		f 4 741 -758 -751 -748
		mu 0 4 530 520 517 531
		f 4 -759 -742 744 742
		mu 0 4 514 520 530 526
		f 4 -743 -783 793 765
		mu 0 4 514 526 527 515
		f 4 -762 -772 -771 -770
		mu 0 4 508 507 538 539
		f 4 -768 -775 773 -773
		mu 0 4 540 541 542 543
		f 4 -765 -780 778 -778
		mu 0 4 512 511 528 544
		f 4 -782 -777 777 780
		mu 0 4 545 522 512 544
		f 4 785 -785 -784 745
		mu 0 4 532 537 546 529
		f 4 784 787 -787 -779
		mu 0 4 546 537 547 548
		f 4 770 -790 -774 -789
		mu 0 4 539 538 543 542
		f 4 -791 769 -792 -788
		mu 0 4 537 508 539 547
		f 4 786 791 788 -793
		mu 0 4 548 547 539 542
		f 4 -794 -781 792 774
		mu 0 4 541 549 548 542
		f 4 -738 -767 772 768
		mu 0 4 535 513 516 536
		f 4 -752 -795 771 759
		mu 0 4 550 534 533 551
		f 4 798 -798 796 795
		mu 0 4 552 553 554 555
		f 4 802 -802 800 799
		mu 0 4 556 557 558 559
		f 4 -807 -806 804 803
		mu 0 4 560 561 562 563
		f 4 -796 -810 -809 -808
		mu 0 4 552 555 564 565
		f 4 -804 -813 811 -811
		mu 0 4 560 563 566 567
		f 4 -816 -815 -803 813
		mu 0 4 568 569 557 556
		f 4 -800 -819 817 -817
		mu 0 4 556 559 570 571
		f 4 -821 -814 816 819
		mu 0 4 572 568 556 571
		f 4 -823 -822 815 820
		mu 0 4 573 574 569 568
		f 4 -801 -825 -824 818
		mu 0 4 559 558 575 570
		f 4 827 826 -826 -818
		mu 0 4 576 577 578 579
		f 4 808 -830 -812 -829
		mu 0 4 565 564 567 566
		f 4 -833 831 829 -831
		mu 0 4 580 581 582 583
		f 4 835 -835 -799 833
		mu 0 4 577 584 553 552
		f 4 -834 807 -837 -827
		mu 0 4 577 552 565 578
		f 4 825 836 828 -838
		mu 0 4 579 578 565 566
		f 4 -839 -820 837 812
		mu 0 4 563 573 579 566
		f 4 -805 -840 822 838
		mu 0 4 563 562 574 573
		f 4 842 841 840 797
		mu 0 4 553 585 586 554
		f 4 845 -845 843 805
		mu 0 4 587 588 589 590
		f 4 847 -847 824 801
		mu 0 4 557 591 575 558
		f 4 -849 -848 814 821
		mu 0 4 592 591 557 569
		f 4 -828 823 849 -836
		mu 0 4 577 576 593 584
		f 4 846 851 -851 -850
		mu 0 4 593 594 595 584
		f 4 852 844 832 -842
		mu 0 4 585 589 588 586
		f 4 850 853 -843 834
		mu 0 4 584 595 585 553
		f 4 854 -853 -854 -852
		mu 0 4 594 589 585 595
		f 4 -844 -855 848 839
		mu 0 4 590 589 594 596
		f 4 -832 -846 806 810
		mu 0 4 582 581 561 560
		f 4 -797 -841 830 809
		mu 0 4 597 598 580 583;
	setAttr ".cd" -type "dataPolyComponent" Index_Data Edge 0 ;
	setAttr ".cvd" -type "dataPolyComponent" Index_Data Vertex 0 ;
	setAttr ".pd[0]" -type "dataPolyComponent" Index_Data UV 0 ;
	setAttr ".hfd" -type "dataPolyComponent" Index_Data Face 0 ;
createNode lightLinker -s -n "lightLinker1";
	rename -uid "9D3FCF18-4319-138F-3B11-5D8B4207D465";
	setAttr -s 4 ".lnk";
	setAttr -s 4 ".slnk";
createNode shapeEditorManager -n "shapeEditorManager";
	rename -uid "1DB95AD1-40B6-E1FC-28E3-078F62A9EFFD";
createNode poseInterpolatorManager -n "poseInterpolatorManager";
	rename -uid "708B4B97-4317-0E85-1ACA-6CBC6880976F";
createNode displayLayerManager -n "layerManager";
	rename -uid "D05CF981-4CE5-9310-003B-878951DC1808";
	setAttr ".cdl" 3;
	setAttr -s 4 ".dli[1:3]"  1 2 3;
	setAttr -s 4 ".dli";
createNode displayLayer -n "defaultLayer";
	rename -uid "C5917737-4971-9889-8803-01B414E4187B";
createNode renderLayerManager -n "renderLayerManager";
	rename -uid "C8599760-447D-768F-43B4-17854751BBC5";
createNode renderLayer -n "defaultRenderLayer";
	rename -uid "42B41ADA-4016-62CE-64AC-6699444CECE2";
	setAttr ".g" yes;
createNode script -n "uiConfigurationScriptNode";
	rename -uid "5A7E9303-475C-0ACC-50DA-4A88666C3FE0";
	setAttr ".b" -type "string" (
		"// Maya Mel UI Configuration File.\n//\n//  This script is machine generated.  Edit at your own risk.\n//\n//\n\nglobal string $gMainPane;\nif (`paneLayout -exists $gMainPane`) {\n\n\tglobal int $gUseScenePanelConfig;\n\tint    $useSceneConfig = $gUseScenePanelConfig;\n\tint    $nodeEditorPanelVisible = stringArrayContains(\"nodeEditorPanel1\", `getPanel -vis`);\n\tint    $nodeEditorWorkspaceControlOpen = (`workspaceControl -exists nodeEditorPanel1Window` && `workspaceControl -q -visible nodeEditorPanel1Window`);\n\tint    $menusOkayInPanels = `optionVar -q allowMenusInPanels`;\n\tint    $nVisPanes = `paneLayout -q -nvp $gMainPane`;\n\tint    $nPanes = 0;\n\tstring $editorName;\n\tstring $panelName;\n\tstring $itemFilterName;\n\tstring $panelConfig;\n\n\t//\n\t//  get current state of the UI\n\t//\n\tsceneUIReplacement -update $gMainPane;\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Top View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Top View\")) -mbv $menusOkayInPanels  $panelName;\n"
		+ "\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|top\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"smoothShaded\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 0\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n            -xray 1\n            -jointXray 1\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n"
		+ "            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n"
		+ "            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -greasePencils 0\n            -shadows 0\n            -captureSequenceNumber -1\n            -width 1\n            -height 1\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n"
		+ "\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Side View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Side View\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|side\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"wireframe\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 0\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n            -xray 1\n            -jointXray 1\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n"
		+ "            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n"
		+ "            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -greasePencils 0\n            -shadows 0\n            -captureSequenceNumber -1\n            -width 1\n            -height 1\n"
		+ "            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Front View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Front View\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|front\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"wireframe\" \n            -activeOnly 0\n            -ignorePanZoom 0\n            -wireframeOnShaded 1\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n"
		+ "            -xray 1\n            -jointXray 1\n            -activeComponentsXray 0\n            -displayTextures 0\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n"
		+ "            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n"
		+ "            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -greasePencils 0\n            -shadows 0\n            -captureSequenceNumber -1\n            -width 1\n            -height 1\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"modelPanel\" (localizedPanelLabel(\"Persp View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tmodelPanel -edit -l (localizedPanelLabel(\"Persp View\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        modelEditor -e \n            -camera \"|persp\" \n            -useInteractiveMode 0\n            -displayLights \"default\" \n            -displayAppearance \"smoothShaded\" \n            -activeOnly 0\n            -ignorePanZoom 0\n"
		+ "            -wireframeOnShaded 0\n            -headsUpDisplay 1\n            -holdOuts 1\n            -selectionHiliteDisplay 1\n            -useDefaultMaterial 0\n            -bufferMode \"double\" \n            -twoSidedLighting 0\n            -backfaceCulling 0\n            -xray 0\n            -jointXray 1\n            -activeComponentsXray 0\n            -displayTextures 1\n            -smoothWireframe 0\n            -lineWidth 1\n            -textureAnisotropic 0\n            -textureHilight 1\n            -textureSampling 2\n            -textureDisplay \"modulate\" \n            -textureMaxSize 32768\n            -fogging 0\n            -fogSource \"fragment\" \n            -fogMode \"linear\" \n            -fogStart 0\n            -fogEnd 100\n            -fogDensity 0.1\n            -fogColor 0.5 0.5 0.5 1 \n            -depthOfFieldPreview 1\n            -maxConstantTransparency 1\n            -rendererName \"vp2Renderer\" \n            -objectFilterShowInHUD 1\n            -isFiltered 0\n            -colorResolution 256 256 \n            -bumpResolution 512 512 \n"
		+ "            -textureCompression 0\n            -transparencyAlgorithm \"frontAndBackCull\" \n            -transpInShadows 0\n            -cullingOverride \"none\" \n            -lowQualityLighting 0\n            -maximumNumHardwareLights 1\n            -occlusionCulling 0\n            -shadingModel 0\n            -useBaseRenderer 0\n            -useReducedRenderer 0\n            -smallObjectCulling 0\n            -smallObjectThreshold -1 \n            -interactiveDisableShadows 0\n            -interactiveBackFaceCull 0\n            -sortTransparent 1\n            -controllers 1\n            -nurbsCurves 1\n            -nurbsSurfaces 1\n            -polymeshes 1\n            -subdivSurfaces 1\n            -planes 1\n            -lights 1\n            -cameras 1\n            -controlVertices 1\n            -hulls 1\n            -grid 1\n            -imagePlane 1\n            -joints 1\n            -ikHandles 1\n            -deformers 1\n            -dynamics 1\n            -particleInstancers 1\n            -fluids 1\n            -hairSystems 1\n            -follicles 1\n"
		+ "            -nCloths 1\n            -nParticles 1\n            -nRigids 1\n            -dynamicConstraints 1\n            -locators 1\n            -manipulators 1\n            -pluginShapes 1\n            -dimensions 1\n            -handles 1\n            -pivots 1\n            -textures 1\n            -strokes 1\n            -motionTrails 1\n            -clipGhosts 1\n            -greasePencils 0\n            -shadows 0\n            -captureSequenceNumber -1\n            -width 1127\n            -height 715\n            -sceneRenderFilter 0\n            $editorName;\n        modelEditor -e -viewSelected 0 $editorName;\n        modelEditor -e \n            -pluginObjects \"gpuCacheDisplayFilter\" 1 \n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"outlinerPanel\" (localizedPanelLabel(\"ToggledOutliner\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\toutlinerPanel -edit -l (localizedPanelLabel(\"ToggledOutliner\")) -mbv $menusOkayInPanels  $panelName;\n"
		+ "\t\t$editorName = $panelName;\n        outlinerEditor -e \n            -docTag \"isolOutln_fromSeln\" \n            -showShapes 0\n            -showAssignedMaterials 0\n            -showTimeEditor 1\n            -showReferenceNodes 1\n            -showReferenceMembers 1\n            -showAttributes 0\n            -showConnected 0\n            -showAnimCurvesOnly 0\n            -showMuteInfo 0\n            -organizeByLayer 1\n            -organizeByClip 1\n            -showAnimLayerWeight 1\n            -autoExpandLayers 1\n            -autoExpand 0\n            -showDagOnly 1\n            -showAssets 1\n            -showContainedOnly 1\n            -showPublishedAsConnected 0\n            -showParentContainers 0\n            -showContainerContents 1\n            -ignoreDagHierarchy 0\n            -expandConnections 0\n            -showUpstreamCurves 1\n            -showUnitlessCurves 1\n            -showCompounds 1\n            -showLeafs 1\n            -showNumericAttrsOnly 0\n            -highlightActive 1\n            -autoSelectNewObjects 0\n"
		+ "            -doNotSelectNewObjects 0\n            -dropIsParent 1\n            -transmitFilters 0\n            -setFilter \"defaultSetFilter\" \n            -showSetMembers 1\n            -allowMultiSelection 1\n            -alwaysToggleSelect 0\n            -directSelect 0\n            -isSet 0\n            -isSetMember 0\n            -displayMode \"DAG\" \n            -expandObjects 0\n            -setsIgnoreFilters 1\n            -containersIgnoreFilters 0\n            -editAttrName 0\n            -showAttrValues 0\n            -highlightSecondary 0\n            -showUVAttrsOnly 0\n            -showTextureNodesOnly 0\n            -attrAlphaOrder \"default\" \n            -animLayerFilterOptions \"allAffecting\" \n            -sortOrder \"none\" \n            -longNames 0\n            -niceNames 1\n            -showNamespace 1\n            -showPinIcons 0\n            -mapMotionTrails 0\n            -ignoreHiddenAttribute 0\n            -ignoreOutlinerColor 0\n            -renderFilterVisible 0\n            -renderFilterIndex 0\n            -selectionOrder \"chronological\" \n"
		+ "            -expandAttribute 0\n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"outlinerPanel\" (localizedPanelLabel(\"Outliner\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\toutlinerPanel -edit -l (localizedPanelLabel(\"Outliner\")) -mbv $menusOkayInPanels  $panelName;\n\t\t$editorName = $panelName;\n        outlinerEditor -e \n            -showShapes 0\n            -showAssignedMaterials 0\n            -showTimeEditor 1\n            -showReferenceNodes 0\n            -showReferenceMembers 0\n            -showAttributes 0\n            -showConnected 0\n            -showAnimCurvesOnly 0\n            -showMuteInfo 0\n            -organizeByLayer 1\n            -organizeByClip 1\n            -showAnimLayerWeight 1\n            -autoExpandLayers 1\n            -autoExpand 0\n            -showDagOnly 1\n            -showAssets 1\n            -showContainedOnly 1\n            -showPublishedAsConnected 0\n            -showParentContainers 0\n"
		+ "            -showContainerContents 1\n            -ignoreDagHierarchy 0\n            -expandConnections 0\n            -showUpstreamCurves 1\n            -showUnitlessCurves 1\n            -showCompounds 1\n            -showLeafs 1\n            -showNumericAttrsOnly 0\n            -highlightActive 1\n            -autoSelectNewObjects 0\n            -doNotSelectNewObjects 0\n            -dropIsParent 1\n            -transmitFilters 0\n            -setFilter \"defaultSetFilter\" \n            -showSetMembers 1\n            -allowMultiSelection 1\n            -alwaysToggleSelect 0\n            -directSelect 0\n            -displayMode \"DAG\" \n            -expandObjects 0\n            -setsIgnoreFilters 1\n            -containersIgnoreFilters 0\n            -editAttrName 0\n            -showAttrValues 0\n            -highlightSecondary 0\n            -showUVAttrsOnly 0\n            -showTextureNodesOnly 0\n            -attrAlphaOrder \"default\" \n            -animLayerFilterOptions \"allAffecting\" \n            -sortOrder \"none\" \n            -longNames 0\n"
		+ "            -niceNames 1\n            -showNamespace 1\n            -showPinIcons 0\n            -mapMotionTrails 0\n            -ignoreHiddenAttribute 0\n            -ignoreOutlinerColor 0\n            -renderFilterVisible 0\n            $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"graphEditor\" (localizedPanelLabel(\"Graph Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Graph Editor\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"OutlineEd\");\n            outlinerEditor -e \n                -showShapes 1\n                -showAssignedMaterials 0\n                -showTimeEditor 1\n                -showReferenceNodes 0\n                -showReferenceMembers 0\n                -showAttributes 1\n                -showConnected 1\n                -showAnimCurvesOnly 1\n                -showMuteInfo 0\n                -organizeByLayer 1\n                -organizeByClip 1\n"
		+ "                -showAnimLayerWeight 1\n                -autoExpandLayers 1\n                -autoExpand 1\n                -showDagOnly 0\n                -showAssets 1\n                -showContainedOnly 0\n                -showPublishedAsConnected 0\n                -showParentContainers 0\n                -showContainerContents 0\n                -ignoreDagHierarchy 0\n                -expandConnections 1\n                -showUpstreamCurves 1\n                -showUnitlessCurves 1\n                -showCompounds 0\n                -showLeafs 1\n                -showNumericAttrsOnly 1\n                -highlightActive 0\n                -autoSelectNewObjects 1\n                -doNotSelectNewObjects 0\n                -dropIsParent 1\n                -transmitFilters 1\n                -setFilter \"0\" \n                -showSetMembers 0\n                -allowMultiSelection 1\n                -alwaysToggleSelect 0\n                -directSelect 0\n                -displayMode \"DAG\" \n                -expandObjects 0\n                -setsIgnoreFilters 1\n"
		+ "                -containersIgnoreFilters 0\n                -editAttrName 0\n                -showAttrValues 0\n                -highlightSecondary 0\n                -showUVAttrsOnly 0\n                -showTextureNodesOnly 0\n                -attrAlphaOrder \"default\" \n                -animLayerFilterOptions \"allAffecting\" \n                -sortOrder \"none\" \n                -longNames 0\n                -niceNames 1\n                -showNamespace 1\n                -showPinIcons 1\n                -mapMotionTrails 1\n                -ignoreHiddenAttribute 0\n                -ignoreOutlinerColor 0\n                -renderFilterVisible 0\n                $editorName;\n\n\t\t\t$editorName = ($panelName+\"GraphEd\");\n            animCurveEditor -e \n                -displayValues 0\n                -snapTime \"integer\" \n                -snapValue \"none\" \n                -showPlayRangeShades \"on\" \n                -lockPlayRangeShades \"off\" \n                -smoothness \"fine\" \n                -resultSamples 1\n                -resultScreenSamples 0\n"
		+ "                -resultUpdate \"delayed\" \n                -showUpstreamCurves 1\n                -keyMinScale 1\n                -stackedCurvesMin -1\n                -stackedCurvesMax 1\n                -stackedCurvesSpace 0.2\n                -preSelectionHighlight 0\n                -constrainDrag 0\n                -valueLinesToggle 0\n                -highlightAffectedCurves 1\n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"dopeSheetPanel\" (localizedPanelLabel(\"Dope Sheet\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Dope Sheet\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"OutlineEd\");\n            outlinerEditor -e \n                -showShapes 1\n                -showAssignedMaterials 0\n                -showTimeEditor 1\n                -showReferenceNodes 0\n                -showReferenceMembers 0\n                -showAttributes 1\n"
		+ "                -showConnected 1\n                -showAnimCurvesOnly 1\n                -showMuteInfo 0\n                -organizeByLayer 1\n                -organizeByClip 1\n                -showAnimLayerWeight 1\n                -autoExpandLayers 1\n                -autoExpand 0\n                -showDagOnly 0\n                -showAssets 1\n                -showContainedOnly 0\n                -showPublishedAsConnected 0\n                -showParentContainers 0\n                -showContainerContents 0\n                -ignoreDagHierarchy 0\n                -expandConnections 1\n                -showUpstreamCurves 1\n                -showUnitlessCurves 0\n                -showCompounds 1\n                -showLeafs 1\n                -showNumericAttrsOnly 1\n                -highlightActive 0\n                -autoSelectNewObjects 0\n                -doNotSelectNewObjects 1\n                -dropIsParent 1\n                -transmitFilters 0\n                -setFilter \"0\" \n                -showSetMembers 0\n                -allowMultiSelection 1\n"
		+ "                -alwaysToggleSelect 0\n                -directSelect 0\n                -displayMode \"DAG\" \n                -expandObjects 0\n                -setsIgnoreFilters 1\n                -containersIgnoreFilters 0\n                -editAttrName 0\n                -showAttrValues 0\n                -highlightSecondary 0\n                -showUVAttrsOnly 0\n                -showTextureNodesOnly 0\n                -attrAlphaOrder \"default\" \n                -animLayerFilterOptions \"allAffecting\" \n                -sortOrder \"none\" \n                -longNames 0\n                -niceNames 1\n                -showNamespace 1\n                -showPinIcons 0\n                -mapMotionTrails 1\n                -ignoreHiddenAttribute 0\n                -ignoreOutlinerColor 0\n                -renderFilterVisible 0\n                $editorName;\n\n\t\t\t$editorName = ($panelName+\"DopeSheetEd\");\n            dopeSheetEditor -e \n                -displayValues 0\n                -snapTime \"integer\" \n                -snapValue \"none\" \n                -outliner \"dopeSheetPanel1OutlineEd\" \n"
		+ "                -showSummary 1\n                -showScene 0\n                -hierarchyBelow 0\n                -showTicks 1\n                -selectionWindow 0 0 0 0 \n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"timeEditorPanel\" (localizedPanelLabel(\"Time Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Time Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"clipEditorPanel\" (localizedPanelLabel(\"Trax Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Trax Editor\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = clipEditorNameFromPanel($panelName);\n            clipEditor -e \n                -displayValues 0\n                -snapTime \"none\" \n"
		+ "                -snapValue \"none\" \n                -initialized 0\n                -manageSequencer 0 \n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"sequenceEditorPanel\" (localizedPanelLabel(\"Camera Sequencer\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Camera Sequencer\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = sequenceEditorNameFromPanel($panelName);\n            clipEditor -e \n                -displayValues 0\n                -snapTime \"none\" \n                -snapValue \"none\" \n                -initialized 0\n                -manageSequencer 1 \n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"hyperGraphPanel\" (localizedPanelLabel(\"Hypergraph Hierarchy\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n"
		+ "\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Hypergraph Hierarchy\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"HyperGraphEd\");\n            hyperGraph -e \n                -graphLayoutStyle \"hierarchicalLayout\" \n                -orientation \"horiz\" \n                -mergeConnections 0\n                -zoom 1\n                -animateTransition 0\n                -showRelationships 1\n                -showShapes 0\n                -showDeformers 0\n                -showExpressions 0\n                -showConstraints 0\n                -showConnectionFromSelected 0\n                -showConnectionToSelected 0\n                -showConstraintLabels 0\n                -showUnderworld 0\n                -showInvisible 0\n                -transitionFrames 1\n                -opaqueContainers 0\n                -freeform 0\n                -imagePosition 0 0 \n                -imageScale 1\n                -imageEnabled 0\n                -graphType \"DAG\" \n                -heatMapDisplay 0\n                -updateSelection 1\n"
		+ "                -updateNodeAdded 1\n                -useDrawOverrideColor 0\n                -limitGraphTraversal -1\n                -range 0 0 \n                -iconSize \"smallIcons\" \n                -showCachedConnections 0\n                $editorName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"hyperShadePanel\" (localizedPanelLabel(\"Hypershade\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Hypershade\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"visorPanel\" (localizedPanelLabel(\"Visor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Visor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"nodeEditorPanel\" (localizedPanelLabel(\"Node Editor\")) `;\n"
		+ "\tif ($nodeEditorPanelVisible || $nodeEditorWorkspaceControlOpen) {\n\t\tif (\"\" == $panelName) {\n\t\t\tif ($useSceneConfig) {\n\t\t\t\t$panelName = `scriptedPanel -unParent  -type \"nodeEditorPanel\" -l (localizedPanelLabel(\"Node Editor\")) -mbv $menusOkayInPanels `;\n\n\t\t\t$editorName = ($panelName+\"NodeEditorEd\");\n            nodeEditor -e \n                -allAttributes 0\n                -allNodes 0\n                -autoSizeNodes 1\n                -consistentNameSize 1\n                -createNodeCommand \"nodeEdCreateNodeCommand\" \n                -connectNodeOnCreation 0\n                -connectOnDrop 0\n                -copyConnectionsOnPaste 0\n                -connectionStyle \"bezier\" \n                -defaultPinnedState 0\n                -additiveGraphingMode 0\n                -connectedGraphingMode 1\n                -settingsChangedCallback \"nodeEdSyncControls\" \n                -traversalDepthLimit -1\n                -keyPressCommand \"nodeEdKeyPressCommand\" \n                -nodeTitleMode \"name\" \n                -gridSnap 0\n"
		+ "                -gridVisibility 1\n                -crosshairOnEdgeDragging 0\n                -popupMenuScript \"nodeEdBuildPanelMenus\" \n                -showNamespace 1\n                -showShapes 1\n                -showSGShapes 0\n                -showTransforms 1\n                -useAssets 1\n                -syncedSelection 1\n                -extendToShapes 1\n                -showUnitConversions 0\n                -editorMode \"default\" \n                -hasWatchpoint 0\n                $editorName;\n\t\t\t}\n\t\t} else {\n\t\t\t$label = `panel -q -label $panelName`;\n\t\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Node Editor\")) -mbv $menusOkayInPanels  $panelName;\n\n\t\t\t$editorName = ($panelName+\"NodeEditorEd\");\n            nodeEditor -e \n                -allAttributes 0\n                -allNodes 0\n                -autoSizeNodes 1\n                -consistentNameSize 1\n                -createNodeCommand \"nodeEdCreateNodeCommand\" \n                -connectNodeOnCreation 0\n                -connectOnDrop 0\n                -copyConnectionsOnPaste 0\n"
		+ "                -connectionStyle \"bezier\" \n                -defaultPinnedState 0\n                -additiveGraphingMode 0\n                -connectedGraphingMode 1\n                -settingsChangedCallback \"nodeEdSyncControls\" \n                -traversalDepthLimit -1\n                -keyPressCommand \"nodeEdKeyPressCommand\" \n                -nodeTitleMode \"name\" \n                -gridSnap 0\n                -gridVisibility 1\n                -crosshairOnEdgeDragging 0\n                -popupMenuScript \"nodeEdBuildPanelMenus\" \n                -showNamespace 1\n                -showShapes 1\n                -showSGShapes 0\n                -showTransforms 1\n                -useAssets 1\n                -syncedSelection 1\n                -extendToShapes 1\n                -showUnitConversions 0\n                -editorMode \"default\" \n                -hasWatchpoint 0\n                $editorName;\n\t\t\tif (!$useSceneConfig) {\n\t\t\t\tpanel -e -l $label $panelName;\n\t\t\t}\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"createNodePanel\" (localizedPanelLabel(\"Create Node\")) `;\n"
		+ "\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Create Node\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"polyTexturePlacementPanel\" (localizedPanelLabel(\"UV Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"UV Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"renderWindowPanel\" (localizedPanelLabel(\"Render View\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Render View\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"shapePanel\" (localizedPanelLabel(\"Shape Editor\")) `;\n"
		+ "\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tshapePanel -edit -l (localizedPanelLabel(\"Shape Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextPanel \"posePanel\" (localizedPanelLabel(\"Pose Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tposePanel -edit -l (localizedPanelLabel(\"Pose Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"dynRelEdPanel\" (localizedPanelLabel(\"Dynamic Relationships\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Dynamic Relationships\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"relationshipPanel\" (localizedPanelLabel(\"Relationship Editor\")) `;\n"
		+ "\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Relationship Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"referenceEditorPanel\" (localizedPanelLabel(\"Reference Editor\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Reference Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"dynPaintScriptedPanelType\" (localizedPanelLabel(\"Paint Effects\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Paint Effects\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"scriptEditorPanel\" (localizedPanelLabel(\"Script Editor\")) `;\n"
		+ "\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Script Editor\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"profilerPanel\" (localizedPanelLabel(\"Profiler Tool\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Profiler Tool\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"contentBrowserPanel\" (localizedPanelLabel(\"Content Browser\")) `;\n\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Content Browser\")) -mbv $menusOkayInPanels  $panelName;\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\t$panelName = `sceneUIReplacement -getNextScriptedPanel \"Stereo\" (localizedPanelLabel(\"Stereo\")) `;\n"
		+ "\tif (\"\" != $panelName) {\n\t\t$label = `panel -q -label $panelName`;\n\t\tscriptedPanel -edit -l (localizedPanelLabel(\"Stereo\")) -mbv $menusOkayInPanels  $panelName;\n{ string $editorName = ($panelName+\"Editor\");\n            stereoCameraView -e \n                -camera \"|persp\" \n                -useInteractiveMode 0\n                -displayLights \"default\" \n                -displayAppearance \"wireframe\" \n                -activeOnly 0\n                -ignorePanZoom 0\n                -wireframeOnShaded 0\n                -headsUpDisplay 1\n                -holdOuts 1\n                -selectionHiliteDisplay 1\n                -useDefaultMaterial 0\n                -bufferMode \"double\" \n                -twoSidedLighting 1\n                -backfaceCulling 0\n                -xray 0\n                -jointXray 0\n                -activeComponentsXray 0\n                -displayTextures 0\n                -smoothWireframe 0\n                -lineWidth 1\n                -textureAnisotropic 0\n                -textureHilight 1\n                -textureSampling 2\n"
		+ "                -textureDisplay \"modulate\" \n                -textureMaxSize 32768\n                -fogging 0\n                -fogSource \"fragment\" \n                -fogMode \"linear\" \n                -fogStart 0\n                -fogEnd 100\n                -fogDensity 0.1\n                -fogColor 0.5 0.5 0.5 1 \n                -depthOfFieldPreview 1\n                -maxConstantTransparency 1\n                -objectFilterShowInHUD 1\n                -isFiltered 0\n                -colorResolution 4 4 \n                -bumpResolution 4 4 \n                -textureCompression 0\n                -transparencyAlgorithm \"frontAndBackCull\" \n                -transpInShadows 0\n                -cullingOverride \"none\" \n                -lowQualityLighting 0\n                -maximumNumHardwareLights 0\n                -occlusionCulling 0\n                -shadingModel 0\n                -useBaseRenderer 0\n                -useReducedRenderer 0\n                -smallObjectCulling 0\n                -smallObjectThreshold -1 \n                -interactiveDisableShadows 0\n"
		+ "                -interactiveBackFaceCull 0\n                -sortTransparent 1\n                -controllers 1\n                -nurbsCurves 1\n                -nurbsSurfaces 1\n                -polymeshes 1\n                -subdivSurfaces 1\n                -planes 1\n                -lights 1\n                -cameras 1\n                -controlVertices 1\n                -hulls 1\n                -grid 1\n                -imagePlane 1\n                -joints 1\n                -ikHandles 1\n                -deformers 1\n                -dynamics 1\n                -particleInstancers 1\n                -fluids 1\n                -hairSystems 1\n                -follicles 1\n                -nCloths 1\n                -nParticles 1\n                -nRigids 1\n                -dynamicConstraints 1\n                -locators 1\n                -manipulators 1\n                -pluginShapes 1\n                -dimensions 1\n                -handles 1\n                -pivots 1\n                -textures 1\n                -strokes 1\n                -motionTrails 1\n"
		+ "                -clipGhosts 1\n                -greasePencils 0\n                -shadows 0\n                -captureSequenceNumber -1\n                -width 0\n                -height 0\n                -sceneRenderFilter 0\n                -displayMode \"centerEye\" \n                -viewColor 0 0 0 1 \n                -useCustomBackground 1\n                $editorName;\n            stereoCameraView -e -viewSelected 0 $editorName;\n            stereoCameraView -e \n                -pluginObjects \"gpuCacheDisplayFilter\" 1 \n                $editorName; };\n\t\tif (!$useSceneConfig) {\n\t\t\tpanel -e -l $label $panelName;\n\t\t}\n\t}\n\n\n\tif ($useSceneConfig) {\n        string $configName = `getPanel -cwl (localizedPanelLabel(\"Current Layout\"))`;\n        if (\"\" != $configName) {\n\t\t\tpanelConfiguration -edit -label (localizedPanelLabel(\"Current Layout\")) \n\t\t\t\t-userCreated false\n\t\t\t\t-defaultImage \"vacantCell.xP:/\"\n\t\t\t\t-image \"\"\n\t\t\t\t-sc false\n\t\t\t\t-configString \"global string $gMainPane; paneLayout -e -cn \\\"single\\\" -ps 1 100 100 $gMainPane;\"\n"
		+ "\t\t\t\t-removeAllPanels\n\t\t\t\t-ap true\n\t\t\t\t\t(localizedPanelLabel(\"Persp View\")) \n\t\t\t\t\t\"modelPanel\"\n"
		+ "\t\t\t\t\t\"$panelName = `modelPanel -unParent -l (localizedPanelLabel(\\\"Persp View\\\")) -mbv $menusOkayInPanels `;\\n$editorName = $panelName;\\nmodelEditor -e \\n    -cam `findStartUpCamera persp` \\n    -useInteractiveMode 0\\n    -displayLights \\\"default\\\" \\n    -displayAppearance \\\"smoothShaded\\\" \\n    -activeOnly 0\\n    -ignorePanZoom 0\\n    -wireframeOnShaded 0\\n    -headsUpDisplay 1\\n    -holdOuts 1\\n    -selectionHiliteDisplay 1\\n    -useDefaultMaterial 0\\n    -bufferMode \\\"double\\\" \\n    -twoSidedLighting 0\\n    -backfaceCulling 0\\n    -xray 0\\n    -jointXray 1\\n    -activeComponentsXray 0\\n    -displayTextures 1\\n    -smoothWireframe 0\\n    -lineWidth 1\\n    -textureAnisotropic 0\\n    -textureHilight 1\\n    -textureSampling 2\\n    -textureDisplay \\\"modulate\\\" \\n    -textureMaxSize 32768\\n    -fogging 0\\n    -fogSource \\\"fragment\\\" \\n    -fogMode \\\"linear\\\" \\n    -fogStart 0\\n    -fogEnd 100\\n    -fogDensity 0.1\\n    -fogColor 0.5 0.5 0.5 1 \\n    -depthOfFieldPreview 1\\n    -maxConstantTransparency 1\\n    -rendererName \\\"vp2Renderer\\\" \\n    -objectFilterShowInHUD 1\\n    -isFiltered 0\\n    -colorResolution 256 256 \\n    -bumpResolution 512 512 \\n    -textureCompression 0\\n    -transparencyAlgorithm \\\"frontAndBackCull\\\" \\n    -transpInShadows 0\\n    -cullingOverride \\\"none\\\" \\n    -lowQualityLighting 0\\n    -maximumNumHardwareLights 1\\n    -occlusionCulling 0\\n    -shadingModel 0\\n    -useBaseRenderer 0\\n    -useReducedRenderer 0\\n    -smallObjectCulling 0\\n    -smallObjectThreshold -1 \\n    -interactiveDisableShadows 0\\n    -interactiveBackFaceCull 0\\n    -sortTransparent 1\\n    -controllers 1\\n    -nurbsCurves 1\\n    -nurbsSurfaces 1\\n    -polymeshes 1\\n    -subdivSurfaces 1\\n    -planes 1\\n    -lights 1\\n    -cameras 1\\n    -controlVertices 1\\n    -hulls 1\\n    -grid 1\\n    -imagePlane 1\\n    -joints 1\\n    -ikHandles 1\\n    -deformers 1\\n    -dynamics 1\\n    -particleInstancers 1\\n    -fluids 1\\n    -hairSystems 1\\n    -follicles 1\\n    -nCloths 1\\n    -nParticles 1\\n    -nRigids 1\\n    -dynamicConstraints 1\\n    -locators 1\\n    -manipulators 1\\n    -pluginShapes 1\\n    -dimensions 1\\n    -handles 1\\n    -pivots 1\\n    -textures 1\\n    -strokes 1\\n    -motionTrails 1\\n    -clipGhosts 1\\n    -greasePencils 0\\n    -shadows 0\\n    -captureSequenceNumber -1\\n    -width 1127\\n    -height 715\\n    -sceneRenderFilter 0\\n    $editorName;\\nmodelEditor -e -viewSelected 0 $editorName;\\nmodelEditor -e \\n    -pluginObjects \\\"gpuCacheDisplayFilter\\\" 1 \\n    $editorName\"\n"
		+ "\t\t\t\t\t\"modelPanel -edit -l (localizedPanelLabel(\\\"Persp View\\\")) -mbv $menusOkayInPanels  $panelName;\\n$editorName = $panelName;\\nmodelEditor -e \\n    -cam `findStartUpCamera persp` \\n    -useInteractiveMode 0\\n    -displayLights \\\"default\\\" \\n    -displayAppearance \\\"smoothShaded\\\" \\n    -activeOnly 0\\n    -ignorePanZoom 0\\n    -wireframeOnShaded 0\\n    -headsUpDisplay 1\\n    -holdOuts 1\\n    -selectionHiliteDisplay 1\\n    -useDefaultMaterial 0\\n    -bufferMode \\\"double\\\" \\n    -twoSidedLighting 0\\n    -backfaceCulling 0\\n    -xray 0\\n    -jointXray 1\\n    -activeComponentsXray 0\\n    -displayTextures 1\\n    -smoothWireframe 0\\n    -lineWidth 1\\n    -textureAnisotropic 0\\n    -textureHilight 1\\n    -textureSampling 2\\n    -textureDisplay \\\"modulate\\\" \\n    -textureMaxSize 32768\\n    -fogging 0\\n    -fogSource \\\"fragment\\\" \\n    -fogMode \\\"linear\\\" \\n    -fogStart 0\\n    -fogEnd 100\\n    -fogDensity 0.1\\n    -fogColor 0.5 0.5 0.5 1 \\n    -depthOfFieldPreview 1\\n    -maxConstantTransparency 1\\n    -rendererName \\\"vp2Renderer\\\" \\n    -objectFilterShowInHUD 1\\n    -isFiltered 0\\n    -colorResolution 256 256 \\n    -bumpResolution 512 512 \\n    -textureCompression 0\\n    -transparencyAlgorithm \\\"frontAndBackCull\\\" \\n    -transpInShadows 0\\n    -cullingOverride \\\"none\\\" \\n    -lowQualityLighting 0\\n    -maximumNumHardwareLights 1\\n    -occlusionCulling 0\\n    -shadingModel 0\\n    -useBaseRenderer 0\\n    -useReducedRenderer 0\\n    -smallObjectCulling 0\\n    -smallObjectThreshold -1 \\n    -interactiveDisableShadows 0\\n    -interactiveBackFaceCull 0\\n    -sortTransparent 1\\n    -controllers 1\\n    -nurbsCurves 1\\n    -nurbsSurfaces 1\\n    -polymeshes 1\\n    -subdivSurfaces 1\\n    -planes 1\\n    -lights 1\\n    -cameras 1\\n    -controlVertices 1\\n    -hulls 1\\n    -grid 1\\n    -imagePlane 1\\n    -joints 1\\n    -ikHandles 1\\n    -deformers 1\\n    -dynamics 1\\n    -particleInstancers 1\\n    -fluids 1\\n    -hairSystems 1\\n    -follicles 1\\n    -nCloths 1\\n    -nParticles 1\\n    -nRigids 1\\n    -dynamicConstraints 1\\n    -locators 1\\n    -manipulators 1\\n    -pluginShapes 1\\n    -dimensions 1\\n    -handles 1\\n    -pivots 1\\n    -textures 1\\n    -strokes 1\\n    -motionTrails 1\\n    -clipGhosts 1\\n    -greasePencils 0\\n    -shadows 0\\n    -captureSequenceNumber -1\\n    -width 1127\\n    -height 715\\n    -sceneRenderFilter 0\\n    $editorName;\\nmodelEditor -e -viewSelected 0 $editorName;\\nmodelEditor -e \\n    -pluginObjects \\\"gpuCacheDisplayFilter\\\" 1 \\n    $editorName\"\n"
		+ "\t\t\t\t$configName;\n\n            setNamedPanelLayout (localizedPanelLabel(\"Current Layout\"));\n        }\n\n        panelHistory -e -clear mainPanelHistory;\n        sceneUIReplacement -clear;\n\t}\n\n\ngrid -spacing 5 -size 12 -divisions 5 -displayAxes yes -displayGridLines yes -displayDivisionLines yes -displayPerspectiveLabels no -displayOrthographicLabels no -displayAxesBold yes -perspectiveLabelPosition axis -orthographicLabelPosition edge;\nviewManip -drawCompass 0 -compassAngle 0 -frontParameters \"\" -homeParameters \"\" -selectionLockParameters \"\";\n}\n");
	setAttr ".st" 3;
createNode script -n "sceneConfigurationScriptNode";
	rename -uid "29E4CD9F-4B74-D831-E073-C68E6D4E8B4B";
	setAttr ".b" -type "string" "playbackOptions -min 24 -max 80 -ast 0 -aet 80 ";
	setAttr ".st" 6;
createNode objectSet -n "textureEditorIsolateSelectSet";
	rename -uid "27DADC38-48C5-2A2A-CD37-1282E8C62078";
	setAttr ".ihi" 0;
	setAttr ".fo" yes;
createNode blinn -n "typeBlinn";
	rename -uid "16CC6EB6-4A54-1B56-0068-749210348654";
	setAttr ".c" -type "float3" 1 1 1 ;
createNode shadingEngine -n "typeBlinnSG";
	rename -uid "82D15B17-4433-83F3-5615-ECBFB6EE2C0B";
	setAttr ".ihi" 0;
	setAttr ".ro" yes;
createNode materialInfo -n "materialInfo1";
	rename -uid "3E4F1287-402A-9101-2FA8-D0A9617B18CD";
createNode surfaceShader -n "surfaceShader1";
	rename -uid "BBCA3361-4F5B-75B5-6CC6-C6B98D35EE9B";
createNode shadingEngine -n "surfaceShader1SG";
	rename -uid "8542B88B-412A-155A-9332-5B852760C135";
	setAttr ".ihi" 0;
	setAttr -s 2 ".dsm";
	setAttr ".ro" yes;
createNode materialInfo -n "materialInfo2";
	rename -uid "34C19350-4136-D751-72D4-7AB74DA96B07";
createNode file -n "file1";
	rename -uid "1CAB6E65-4CE2-3652-5261-43ACF2113061";
	setAttr ".ftn" -type "string" "D:/Work/Animation/Personal Work/GameJam2097_Rabbits&Supermarkets/GameJam2097_Rabbits&Supermarkets//images/rabbitTEX.jpg";
	setAttr ".ft" 0;
	setAttr ".cs" -type "string" "sRGB";
createNode place2dTexture -n "place2dTexture1";
	rename -uid "593F1598-4F8E-6E0E-6D1F-28BC00137A9A";
createNode skinCluster -n "skinCluster1";
	rename -uid "52378C3D-4513-50E8-35FB-B780A5CB6D15";
	setAttr -s 435 ".wl";
	setAttr ".wl[0:434].w"
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 2 1
		1 22 1
		1 22 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 22 1
		1 22 1
		1 22 1
		1 23 1
		1 23 1
		1 22 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 25 1
		1 25 1
		1 26 1
		1 26 1
		1 26 1
		1 26 1
		1 25 1
		1 25 1
		1 2 1
		1 26 1
		1 26 1
		1 25 1
		1 26 1
		1 26 1
		1 26 1
		1 26 1
		1 26 1
		1 26 1
		1 1 1
		1 11 1
		1 1 1
		1 11 1
		1 1 1
		1 11 1
		1 1 1
		1 11 1
		1 1 1
		1 0 1
		1 11 1
		1 11 1
		1 1 1
		1 1 1
		1 11 1
		1 11 1
		1 11 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 1 1
		1 1 1
		1 1 1
		1 10 1
		1 10 1
		1 10 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 1 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 1 1
		1 1 1
		1 1 1
		1 0 1
		1 0 1
		1 1 1
		1 1 1
		1 1 1
		1 0 1
		1 1 1
		1 0 1
		1 1 1
		1 1 1
		1 1 1
		1 0 1
		1 1 1
		1 1 1
		1 10 1
		1 10 1
		1 10 1
		1 10 1
		1 10 1
		1 10 1
		1 10 1
		1 10 1
		1 11 1
		1 11 1
		1 1 1
		1 14 1
		1 1 1
		1 14 1
		1 1 1
		1 14 1
		1 1 1
		1 14 1
		1 1 1
		1 0 1
		1 14 1
		1 14 1
		1 1 1
		1 1 1
		1 14 1
		1 14 1
		1 14 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 0 1
		1 1 1
		1 1 1
		1 1 1
		1 13 1
		1 13 1
		1 13 1
		1 0 1
		1 0 1
		1 0 1
		1 1 1
		1 0 1
		1 0 1
		1 0 1
		1 1 1
		1 1 1
		1 1 1
		1 0 1
		1 0 1
		1 1 1
		1 13 1
		1 13 1
		1 13 1
		1 13 1
		1 13 1
		1 13 1
		1 13 1
		1 13 1
		1 14 1
		1 14 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 23 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 22 1
		1 16 1
		1 16 1
		1 17 1
		1 16 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 16 1
		1 16 1
		1 17 1
		1 16 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 17 1
		1 19 1
		1 19 1
		1 20 1
		1 19 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 19 1
		1 19 1
		1 20 1
		1 19 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1
		1 20 1;
	setAttr -s 28 ".pm";
	setAttr ".pm[0]" -type "matrix" 0 -1 0 0 1 0 0 0 0 0 1 0 -0.80927163362503052 1.2554342668302078e-17 0.0008316162105935776 1;
	setAttr ".pm[1]" -type "matrix" 0 -1 0 0 1 0 0 0 0 0 1 0 -1.6884008890535362 1.2554342668302078e-17 0.0008316162105935776 1;
	setAttr ".pm[2]" -type "matrix" 0 -1 0 0 1 0 0 0 0 0 1 0 -2.224403264145324 1.2554342668302078e-17 0.0008316162105935776 1;
	setAttr ".pm[3]" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 -1.2554342668302079e-17 -3.7740431127748075 0.0008316162105935776 1;
	setAttr ".pm[4]" -type "matrix" 0.38857188131929182 -0.90418430108850845 0.17737712003827244 0
		 0.91875300822529327 0.39483276190936106 4.7184478546569153e-16 0 -0.0700342982042398 0.16296576262550161 0.98414295571676391 0
		 -3.2464899733034684 -0.91280494713725435 0.0042361564761981274 1;
	setAttr ".pm[5]" -type "matrix" 0.38208303096425444 -0.80503054254882045 -0.4538043444180771 0
		 0.90341075348396505 0.42877617761430598 8.3266726846886765e-17 0 0.194580492184349 -0.40997172472503179 0.89110135056977646 0
		 -4.1604117466860577 -1.2295763402157207 0.51630234087180138 1;
	setAttr ".pm[6]" -type "matrix" 1.0000000000000002 1.0768041740462307e-16 -2.2204460492503136e-16 0
		 1.8488927466117472e-32 1.0000000000000002 1.1102230246251568e-16 0 1.6653345369377351e-16 -7.4685276079902064e-17 1 0
		 -1.1415039332005683 -5.0126662254333514 -0.0019269448798842881 1;
	setAttr ".pm[7]" -type "matrix" 0.38857188131929199 -0.90418430108850856 0.17737712003827288 0
		 -0.91875300822529338 -0.39483276190936112 -5.5511151231257839e-17 0 0.070034298204239606 -0.16296576262550211 -0.98414295571676402 0
		 3.2464919011035227 0.9128062168377592 -0.0042362538008471626 1;
	setAttr ".pm[8]" -type "matrix" 0.38208303096425444 -0.80503054254882067 -0.45380434441807643 0
		 -0.90341075348396505 -0.42877617761430586 -3.8857805861880479e-16 0 -0.19458049218434842 0.40997172472503146 -0.89110135056977646 0
		 4.1604082312650803 1.2295749916082439 -0.51630187861885657 1;
	setAttr ".pm[9]" -type "matrix" 1 -1.2406869261560826e-16 -3.8857805861880474e-16 0
		 -1.2361790325871277e-16 -1 -4.7486733016439186e-16 0 -6.106226635438359e-16 5.769274786535053e-16 -0.99999999999999978 0
		 1.1415000000000004 5.01267 0.0019269400000017855 1;
	setAttr ".pm[10]" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 -0.40845635520577289 -1.8094612670184995 0.0008316162105935776 1;
	setAttr ".pm[11]" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 -0.85055884512012658 -1.8094612670184995 0.0008316162105935776 1;
	setAttr ".pm[12]" -type "matrix" 0 -1 0 0 1 0 0 0 0 0 1 0 -1.8094612670184995 1.5081021423023888 0.0008316162105935776 1;
	setAttr ".pm[13]" -type "matrix" 1 0 0 0 0 -1 1.2246467991473532e-16 0 0 -1.2246467991473532e-16 -1 0
		 0.40845599999999999 1.8094600000000001 -0.00083161600000022159 1;
	setAttr ".pm[14]" -type "matrix" 1 0 0 0 0 -1 1.2246467991473532e-16 0 0 -1.2246467991473532e-16 -1 0
		 0.85055899999999995 1.8094600000000001 -0.00083161600000022159 1;
	setAttr ".pm[15]" -type "matrix" 0 -1 0 0 -1 0 1.2246467991473532e-16 0 -1.2246467991473532e-16 0 -1 0
		 1.8094600000000001 -1.5081 -0.00083161600000022159 1;
	setAttr ".pm[16]" -type "matrix" 0 -1 0 0 -1 0 1.2246467991473532e-16 0 -1.2246467991473532e-16 0 -1 0
		 0.54792957913111406 0.29430170637130593 0.00066644006628662161 1;
	setAttr ".pm[17]" -type "matrix" 0 -1 0 0 -1 0 1.2246467991473532e-16 0 -1.2246467991473532e-16 0 -1 0
		 0.30389837187225333 0.29430170637130593 0.00066644006628665154 1;
	setAttr ".pm[18]" -type "matrix" 0 -1 0 0 1 0 -2.4492935982947064e-16 0 2.4492935982947064e-16 0 1 0
		 -0.16663081778914418 0.29430170637130593 -0.33772910236433168 1;
	setAttr ".pm[19]" -type "matrix" 0 -1 0 0 1 0 0 0 0 0 1 0 -0.54793000000000003 -0.29430200000000001 -0.00066643999999999991 1;
	setAttr ".pm[20]" -type "matrix" 0 -1 0 0 1 0 0 0 0 0 1 0 -0.303898 -0.29430200000000001 -0.00066644000000000002 1;
	setAttr ".pm[21]" -type "matrix" 0 -1 0 0 -1 0 1.2246467991473532e-16 0 -1.2246467991473532e-16 0 -1 0
		 0.16663100000000003 -0.29430200000000001 0.337729 1;
	setAttr ".pm[22]" -type "matrix" 0.39250010579866973 -0.91974954629554095 0.0021069022710322322 0
		 0.91802556418698344 0.39162205864115562 -0.062138769579337685 0 0.056326995723221633 0.026323663780140404 0.99806529559843238 0
		 -3.2353083609892188 -0.90660510938534389 0.2894390233702806 1;
	setAttr ".pm[23]" -type "matrix" 0.38758303580364317 -0.92147374193802634 -0.025797931621809427 0
		 0.91749330548532115 0.38831944049608014 -0.086104857716799724 0 0.089361203812677967 0.0097033525929397686 0.99595201701769065 0
		 -4.2423182601228939 -0.8935080058067143 0.54016965911705839 1;
	setAttr ".pm[24]" -type "matrix" 0.99999966392981798 0.00081706079500782399 6.7467832378598634e-05 0
		 -0.00081673337538258942 0.99998853567882129 -0.0047182049044439522 0 -7.1322119155940478e-05 0.0047181482155655196 0.99998886693331335 0
		 -1.1387792468331903 -5.0125930844734548 0.013816319724877135 1;
	setAttr ".pm[25]" -type "matrix" 0.39250010579867001 -0.91974954629554095 0.0021069022710314242 0
		 -0.91802556418698344 -0.39162205864115596 0.062138769579337512 0 -0.056326995723221786 -0.026323663780139603 -0.9980652955984326 0
		 3.2353102824841908 0.9066063815305182 -0.28943918928661977 1;
	setAttr ".pm[26]" -type "matrix" 0.38758303580364317 -0.92147374193802678 -0.025797931621801155 0
		 -0.91749330548532149 -0.38831944049607953 0.086104857716803082 0 -0.089361203812677828 -0.0097033525929486417 -0.99595201701769054 0
		 4.2423217516897482 0.89350913061586468 -0.54016961022082333 1;
	setAttr ".pm[27]" -type "matrix" 0.99999966392981787 0.00081706079500767285 6.7467832378593389e-05 0
		 0.00081673337538237128 -0.99998853567882107 0.0047182049044554837 0 7.1322119155934989e-05 -0.0047181482155770511 -0.99998886693331313 0
		 1.1387757221718446 5.012592353890641 -0.013816317590160714 1;
	setAttr ".gm" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1;
	setAttr -s 15 ".ma";
	setAttr -s 28 ".dpf[0:27]"  4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 4 
		4 4 4 4 4 4 4 4;
	setAttr -s 15 ".lw";
	setAttr ".mmi" yes;
	setAttr ".mi" 4;
	setAttr ".ucm" yes;
	setAttr -s 15 ".ifcl";
createNode groupId -n "groupId56";
	rename -uid "51C2DAE5-4358-246F-721A-D683860D1B60";
	setAttr ".ihi" 0;
createNode groupParts -n "skinCluster1GroupParts";
	rename -uid "C46E4609-4E4E-E5BB-8730-FB83D41DB297";
	setAttr ".ihi" 0;
	setAttr ".ic" -type "componentList" 4 "f[150]" "f[154]" "f[165]" "f[169]";
createNode dagPose -n "bindPose1";
	rename -uid "1365A342-476C-DF49-051D-04B43D953504";
	setAttr -s 16 ".wm";
	setAttr ".wm[22]" -type "matrix" 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1;
	setAttr -s 29 ".xm";
	setAttr ".xm[0]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.2554342668302078e-17
		 0.80927163362503052 -0.0008316162105935776 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 
		0 0 0.70710678118654757 0.70710678118654757 1 1 1 no;
	setAttr ".xm[1]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.87912925542850573 0
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 1 no;
	setAttr ".xm[2]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.53600237509178772 0
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 1 no;
	setAttr ".xm[3]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.5496398486294836 -3.0814879110195774e-33
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 -0.70710678118654757 0.70710678118654757 1
		 1 1 no;
	setAttr ".xm[4]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.1187244633632205 -0.43539941621753542
		 -0.081947059913627052 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0.087214894685513661 0.017946706845904051 -0.20075211481013233 0.97558703674128511 1
		 1 1 no;
	setAttr ".xm[5]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.0260059212564514 4.163336342344337e-17
		 1.3877787807814457e-17 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.29068653473792083 -0.13138886506267219 -0.017651783210531225 0.94758995309632277 1
		 1 1 no;
	setAttr ".xm[6]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.80460905353433854 -1.2490009027033011e-16
		 -1.3877787807814457e-16 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0.12470515504598442 0.1972256297241026 -0.51967348931593971 0.82188207170316363 1
		 1 1 no;
	setAttr ".xm[7]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.118726735854676 0.43539899999999998
		 -0.081947083789406419 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.20075211481013236 0.97558703674128511 -0.087214894685513883 -0.017946706845903892 1
		 1 1 no;
	setAttr ".xm[8]" -type "matrix" "xform" 1 1 1 0 0 0 0 -1.0260003431099771 2.2836820072758002e-06
		 -2.9075958385502837e-07 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.29068653473792083 -0.13138886506267181 -0.017651783210531138 0.94758995309632288 1
		 1 1 no;
	setAttr ".xm[9]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.80461447518069296 -6.1353989384560492e-06
		 -1.3183020912688193e-06 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0.12470515504598415 0.19722562972410221 -0.5196734893159396 0.82188207170316374 1
		 1 1 no;
	setAttr ".xm[10]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.12106037796496327 -0.40845635520577289
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 -0.70710678118654757 0.70710678118654757 1
		 1 1 no;
	setAttr ".xm[11]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.44210248991435369 0
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 1 no;
	setAttr ".xm[12]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.65754329718226223 0
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0.70710678118654757 0.70710678118654757 1
		 1 1 no;
	setAttr ".xm[13]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.12105911094646382 0.40845599999999999
		 2.1059357762417269e-10 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.70710678118654757 0.70710678118654757 -4.3297802811774664e-17 4.3297802811774664e-17 1
		 1 1 no;
	setAttr ".xm[14]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.44210299999999997
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 1 no;
	setAttr ".xm[15]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.65754100000000004
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0.70710678118654757 0.70710678118654757 1
		 1 1 no;
	setAttr ".xm[16]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.26134205449391645
		 -0.29430170637130593 0.0014980562768802663 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 
		0 1 0 6.123233995736766e-17 1 1 1 no;
	setAttr ".xm[17]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.24403120725886074 0
		 -2.9885203686162816e-17 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 1 no;
	setAttr ".xm[18]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.1372675540831092 0
		 -0.33706266229804505 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 1 0 6.123233995736766e-17 1
		 1 1 no;
	setAttr ".xm[19]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.26134163362503049
		 0.29430200000000001 0.0014980562105935775 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 
		0 0 0 1 1 1 1 no;
	setAttr ".xm[20]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.24403200000000003
		 0 1.0842021724855044e-19 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 1 no;
	setAttr ".xm[21]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.137267 0 0.33706256000000001 0
		 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 1 0 6.123233995736766e-17 1 1 1 no;
	setAttr ".xm[22]" -type "matrix" "xform" 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
		 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 1 1 1 yes;
	setAttr ".xm[23]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.1187244633632205 -0.43539941621753542
		 -0.081947059913627052 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0.0072581384150841068 -0.030243538607961657 -0.20018128353813955 0.97926508235525245 1
		 1 1 yes;
	setAttr ".xm[24]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.0183582287043036 1.1102230246251565e-15
		 -0.12503866795331797 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.0081540954586150384 -0.01651939761739725 0.0024575287481689921 0.99982727547415007 1
		 1 1 yes;
	setAttr ".xm[25]" -type "matrix" "xform" 1 1 1 0 0 0 0 0.7996859916651351 -0.00041214217747687076
		 0.088869983137121522 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.026804222615424582 0.033298787147942627 -0.55270027900984298 0.83228308045213839 1
		 1 1 yes;
	setAttr ".xm[26]" -type "matrix" "xform" 1 1 1 0 0 0 0 1.118726735854676 0.43539899999999998
		 -0.081947083789406419 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.20018128353813969 0.97926508235525245 -0.0072581384150836965 0.030243538607961654 1
		 1 1 yes;
	setAttr ".xm[27]" -type "matrix" "xform" 1 1 1 0 0 0 0 -1.0183597899867038 1.2860695663974298e-07
		 0.12503835640838384 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 -0.008154095458610176 -0.0165193976173971 0.0024575287481691907 0.99982727547415007 1
		 1 1 yes;
	setAttr ".xm[28]" -type "matrix" "xform" 1 1 1 0 0 0 0 -0.79968046600848997
		 0.00041030114752271807 -0.088870086787654567 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 
		-0.026804222615423451 0.033298787147936916 -0.5527002790098432 0.83228308045213828 1
		 1 1 yes;
	setAttr -s 18 ".m";
	setAttr -s 20 ".p";
	setAttr -s 25 ".g[4:28]" yes yes no yes yes no no no no no no no no 
		no no no no no yes no no no no no no;
	setAttr ".bp" yes;
createNode displayLayer -n "GEO_LYR";
	rename -uid "2BB36194-4AC6-3FAA-6999-F19B9F962E5C";
	setAttr ".do" 1;
createNode displayLayer -n "JNT_LYR";
	rename -uid "2F5BBEC8-4A63-08B9-0D28-ADA2AF9A5926";
	setAttr ".v" no;
	setAttr ".do" 2;
createNode makeNurbCircle -n "makeNurbCircle1";
	rename -uid "3D159CDA-4A29-A302-C6FA-FEA90FC9FE37";
	setAttr ".nr" -type "double3" 0 1 0 ;
	setAttr ".tol" 0;
createNode makeNurbCircle -n "makeNurbCircle2";
	rename -uid "03AB55DE-4951-B3CA-BAF6-9082BF4DF903";
	setAttr ".nr" -type "double3" 0 1 0 ;
	setAttr ".tol" 0;
createNode makeNurbCircle -n "makeNurbCircle3";
	rename -uid "859C8D99-4A64-1F86-BF29-D1B78062D5C8";
	setAttr ".nr" -type "double3" 0 1 0 ;
	setAttr ".tol" 0;
createNode displayLayer -n "CTRL_LYR";
	rename -uid "607E7B52-42A5-E99B-473E-52B8979DC982";
	setAttr ".do" 3;
createNode transformGeometry -n "transformGeometry1";
	rename -uid "AAF44CCC-4475-4C76-281B-DFB7D16E653D";
	setAttr ".txf" -type "matrix" 0.75158093538691673 0 0 0 0 0.75158093538691673 0 0
		 0 0 0.75158093538691673 0 1.2554342668302079e-17 1.6884008646011353 -0.00083161622751504204 1;
createNode transformGeometry -n "transformGeometry2";
	rename -uid "64A71D58-4014-8FEC-8C19-BDBDFC3E5317";
	setAttr ".txf" -type "matrix" -1.0834459313586815e-16 -0.2439703346371519 0 0 0.2439703346371519 -1.0834459313586815e-16 0 0
		 0 0 0.2439703346371519 0 0.40845635533332825 1.8094612360000606 -0.00083161622751504172 1;
createNode nodeGraphEditorInfo -n "hyperShadePrimaryNodeEditorSavedTabsInfo";
	rename -uid "EBBE1EF2-4992-8EE9-0452-FDAF3BFE2612";
	setAttr ".tgi[0].tn" -type "string" "Untitled_1";
	setAttr ".tgi[0].vl" -type "double2" -749.0842193183164 -426.1904592551889 ;
	setAttr ".tgi[0].vh" -type "double2" 987.17944795250025 44.04761729732396 ;
	setAttr -s 4 ".tgi[0].ni";
	setAttr ".tgi[0].ni[0].x" 178.57142639160156;
	setAttr ".tgi[0].ni[0].y" -137.14285278320312;
	setAttr ".tgi[0].ni[0].nvs" 1923;
	setAttr ".tgi[0].ni[1].x" -128.57142639160156;
	setAttr ".tgi[0].ni[1].y" -91.428573608398438;
	setAttr ".tgi[0].ni[1].nvs" 1923;
	setAttr ".tgi[0].ni[2].x" 485.71429443359375;
	setAttr ".tgi[0].ni[2].y" -114.28571319580078;
	setAttr ".tgi[0].ni[2].nvs" 1923;
	setAttr ".tgi[0].ni[3].x" -435.71429443359375;
	setAttr ".tgi[0].ni[3].y" -114.28571319580078;
	setAttr ".tgi[0].ni[3].nvs" 1923;
select -ne :time1;
	setAttr ".o" 24;
	setAttr ".unw" 24;
select -ne :hardwareRenderingGlobals;
	setAttr ".otfna" -type "stringArray" 22 "NURBS Curves" "NURBS Surfaces" "Polygons" "Subdiv Surface" "Particles" "Particle Instance" "Fluids" "Strokes" "Image Planes" "UI" "Lights" "Cameras" "Locators" "Joints" "IK Handles" "Deformers" "Motion Trails" "Components" "Hair Systems" "Follicles" "Misc. UI" "Ornaments"  ;
	setAttr ".otfva" -type "Int32Array" 22 0 1 1 1 1 1
		 1 1 1 0 0 0 0 0 0 0 0 0
		 0 0 0 0 ;
	setAttr ".fprt" yes;
select -ne :renderPartition;
	setAttr -s 4 ".st";
select -ne :renderGlobalsList1;
select -ne :defaultShaderList1;
	setAttr -s 7 ".s";
select -ne :postProcessList1;
	setAttr -s 2 ".p";
select -ne :defaultRenderUtilityList1;
select -ne :defaultRenderingList1;
select -ne :defaultTextureList1;
select -ne :initialShadingGroup;
	setAttr ".ro" yes;
select -ne :initialParticleSE;
	setAttr ".ro" yes;
select -ne :defaultRenderGlobals;
	addAttr -ci true -h true -sn "dss" -ln "defaultSurfaceShader" -dt "string";
	setAttr ".ren" -type "string" "arnold";
	setAttr ".dss" -type "string" "lambert1";
select -ne :defaultResolution;
	setAttr ".pa" 1;
select -ne :defaultColorMgtGlobals;
	setAttr ".cfe" yes;
	setAttr ".cfp" -type "string" "<MAYA_RESOURCES>/OCIO-configs/Maya2022-default/config.ocio";
	setAttr ".vtn" -type "string" "ACES 1.0 SDR-video (sRGB)";
	setAttr ".vn" -type "string" "ACES 1.0 SDR-video";
	setAttr ".dn" -type "string" "sRGB";
	setAttr ".wsn" -type "string" "ACEScg";
	setAttr ".otn" -type "string" "ACES 1.0 SDR-video (sRGB)";
	setAttr ".potn" -type "string" "ACES 1.0 SDR-video (sRGB)";
select -ne :hardwareRenderGlobals;
	setAttr ".ctrs" 256;
	setAttr ".btrs" 512;
connectAttr "makeNurbCircle2.oc" "root_CTRLShape.cr";
connectAttr "CTRL_LYR.di" "CTRL_GRP.do";
connectAttr "transformGeometry1.og" "chest_CTRLShape.cr";
connectAttr "transformGeometry2.og" "shoulder_L_CTRLShape.cr";
connectAttr "leg_R_CTRL_0_pointConstraint1.ctx" "leg_R_CTRL_0.tx";
connectAttr "leg_R_CTRL_0_pointConstraint1.cty" "leg_R_CTRL_0.ty";
connectAttr "leg_R_CTRL_0_pointConstraint1.ctz" "leg_R_CTRL_0.tz";
connectAttr "leg_R_CTRL_0.pim" "leg_R_CTRL_0_pointConstraint1.cpim";
connectAttr "leg_R_CTRL_0.rp" "leg_R_CTRL_0_pointConstraint1.crp";
connectAttr "leg_R_CTRL_0.rpt" "leg_R_CTRL_0_pointConstraint1.crt";
connectAttr "hip_CTRL.t" "leg_R_CTRL_0_pointConstraint1.tg[0].tt";
connectAttr "hip_CTRL.rp" "leg_R_CTRL_0_pointConstraint1.tg[0].trp";
connectAttr "hip_CTRL.rpt" "leg_R_CTRL_0_pointConstraint1.tg[0].trt";
connectAttr "hip_CTRL.pm" "leg_R_CTRL_0_pointConstraint1.tg[0].tpm";
connectAttr "leg_R_CTRL_0_pointConstraint1.w0" "leg_R_CTRL_0_pointConstraint1.tg[0].tw"
		;
connectAttr "leg_L_CTRL_0_pointConstraint1.ctx" "leg_L_CTRL_0.tx";
connectAttr "leg_L_CTRL_0_pointConstraint1.cty" "leg_L_CTRL_0.ty";
connectAttr "leg_L_CTRL_0_pointConstraint1.ctz" "leg_L_CTRL_0.tz";
connectAttr "leg_L_CTRL_0.pim" "leg_L_CTRL_0_pointConstraint1.cpim";
connectAttr "leg_L_CTRL_0.rp" "leg_L_CTRL_0_pointConstraint1.crp";
connectAttr "leg_L_CTRL_0.rpt" "leg_L_CTRL_0_pointConstraint1.crt";
connectAttr "hip_CTRL.t" "leg_L_CTRL_0_pointConstraint1.tg[0].tt";
connectAttr "hip_CTRL.rp" "leg_L_CTRL_0_pointConstraint1.tg[0].trp";
connectAttr "hip_CTRL.rpt" "leg_L_CTRL_0_pointConstraint1.tg[0].trt";
connectAttr "hip_CTRL.pm" "leg_L_CTRL_0_pointConstraint1.tg[0].tpm";
connectAttr "leg_L_CTRL_0_pointConstraint1.w0" "leg_L_CTRL_0_pointConstraint1.tg[0].tw"
		;
connectAttr "JNT_LYR.di" "JNT_GRP.do";
connectAttr "pelvis_JNT_parentConstraint1.ctx" "pelvis_JNT.tx";
connectAttr "pelvis_JNT_parentConstraint1.cty" "pelvis_JNT.ty";
connectAttr "pelvis_JNT_parentConstraint1.ctz" "pelvis_JNT.tz";
connectAttr "pelvis_JNT_parentConstraint1.crx" "pelvis_JNT.rx";
connectAttr "pelvis_JNT_parentConstraint1.cry" "pelvis_JNT.ry";
connectAttr "pelvis_JNT_parentConstraint1.crz" "pelvis_JNT.rz";
connectAttr "pelvis_JNT.s" "chest_JNT.is";
connectAttr "chest_JNT_parentConstraint1.ctx" "chest_JNT.tx";
connectAttr "chest_JNT_parentConstraint1.cty" "chest_JNT.ty";
connectAttr "chest_JNT_parentConstraint1.ctz" "chest_JNT.tz";
connectAttr "chest_JNT_parentConstraint1.crx" "chest_JNT.rx";
connectAttr "chest_JNT_parentConstraint1.cry" "chest_JNT.ry";
connectAttr "chest_JNT_parentConstraint1.crz" "chest_JNT.rz";
connectAttr "chest_JNT.s" "head_JNT.is";
connectAttr "head_JNT_parentConstraint1.ctx" "head_JNT.tx";
connectAttr "head_JNT_parentConstraint1.cty" "head_JNT.ty";
connectAttr "head_JNT_parentConstraint1.ctz" "head_JNT.tz";
connectAttr "head_JNT_parentConstraint1.crx" "head_JNT.rx";
connectAttr "head_JNT_parentConstraint1.cry" "head_JNT.ry";
connectAttr "head_JNT_parentConstraint1.crz" "head_JNT.rz";
connectAttr "head_JNT.s" "headTIP_JNT.is";
connectAttr "head_JNT.s" "ear1_L_JNT.is";
connectAttr "ear1_L_JNT_parentConstraint1.ctx" "ear1_L_JNT.tx";
connectAttr "ear1_L_JNT_parentConstraint1.cty" "ear1_L_JNT.ty";
connectAttr "ear1_L_JNT_parentConstraint1.ctz" "ear1_L_JNT.tz";
connectAttr "ear1_L_JNT_parentConstraint1.crx" "ear1_L_JNT.rx";
connectAttr "ear1_L_JNT_parentConstraint1.cry" "ear1_L_JNT.ry";
connectAttr "ear1_L_JNT_parentConstraint1.crz" "ear1_L_JNT.rz";
connectAttr "ear1_L_JNT.s" "ear2_L_JNT.is";
connectAttr "ear2_L_JNT_parentConstraint1.ctx" "ear2_L_JNT.tx";
connectAttr "ear2_L_JNT_parentConstraint1.cty" "ear2_L_JNT.ty";
connectAttr "ear2_L_JNT_parentConstraint1.ctz" "ear2_L_JNT.tz";
connectAttr "ear2_L_JNT_parentConstraint1.crx" "ear2_L_JNT.rx";
connectAttr "ear2_L_JNT_parentConstraint1.cry" "ear2_L_JNT.ry";
connectAttr "ear2_L_JNT_parentConstraint1.crz" "ear2_L_JNT.rz";
connectAttr "ear2_L_JNT.s" "|root_CTRL|JNT_GRP|pelvis_JNT|chest_JNT|head_JNT|ear1_L_JNT|ear2_L_JNT|earTIP_JNT.is"
		;
connectAttr "ear2_L_JNT.ro" "ear2_L_JNT_parentConstraint1.cro";
connectAttr "ear2_L_JNT.pim" "ear2_L_JNT_parentConstraint1.cpim";
connectAttr "ear2_L_JNT.rp" "ear2_L_JNT_parentConstraint1.crp";
connectAttr "ear2_L_JNT.rpt" "ear2_L_JNT_parentConstraint1.crt";
connectAttr "ear2_L_JNT.jo" "ear2_L_JNT_parentConstraint1.cjo";
connectAttr "ear2_L_CTRL.t" "ear2_L_JNT_parentConstraint1.tg[0].tt";
connectAttr "ear2_L_CTRL.rp" "ear2_L_JNT_parentConstraint1.tg[0].trp";
connectAttr "ear2_L_CTRL.rpt" "ear2_L_JNT_parentConstraint1.tg[0].trt";
connectAttr "ear2_L_CTRL.r" "ear2_L_JNT_parentConstraint1.tg[0].tr";
connectAttr "ear2_L_CTRL.ro" "ear2_L_JNT_parentConstraint1.tg[0].tro";
connectAttr "ear2_L_CTRL.s" "ear2_L_JNT_parentConstraint1.tg[0].ts";
connectAttr "ear2_L_CTRL.pm" "ear2_L_JNT_parentConstraint1.tg[0].tpm";
connectAttr "ear2_L_JNT_parentConstraint1.w0" "ear2_L_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "ear1_L_JNT.ro" "ear1_L_JNT_parentConstraint1.cro";
connectAttr "ear1_L_JNT.pim" "ear1_L_JNT_parentConstraint1.cpim";
connectAttr "ear1_L_JNT.rp" "ear1_L_JNT_parentConstraint1.crp";
connectAttr "ear1_L_JNT.rpt" "ear1_L_JNT_parentConstraint1.crt";
connectAttr "ear1_L_JNT.jo" "ear1_L_JNT_parentConstraint1.cjo";
connectAttr "ear1_L_CTRL.t" "ear1_L_JNT_parentConstraint1.tg[0].tt";
connectAttr "ear1_L_CTRL.rp" "ear1_L_JNT_parentConstraint1.tg[0].trp";
connectAttr "ear1_L_CTRL.rpt" "ear1_L_JNT_parentConstraint1.tg[0].trt";
connectAttr "ear1_L_CTRL.r" "ear1_L_JNT_parentConstraint1.tg[0].tr";
connectAttr "ear1_L_CTRL.ro" "ear1_L_JNT_parentConstraint1.tg[0].tro";
connectAttr "ear1_L_CTRL.s" "ear1_L_JNT_parentConstraint1.tg[0].ts";
connectAttr "ear1_L_CTRL.pm" "ear1_L_JNT_parentConstraint1.tg[0].tpm";
connectAttr "ear1_L_JNT_parentConstraint1.w0" "ear1_L_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "head_JNT.s" "ear1_R_JNT.is";
connectAttr "ear1_R_JNT_parentConstraint1.ctx" "ear1_R_JNT.tx";
connectAttr "ear1_R_JNT_parentConstraint1.cty" "ear1_R_JNT.ty";
connectAttr "ear1_R_JNT_parentConstraint1.ctz" "ear1_R_JNT.tz";
connectAttr "ear1_R_JNT_parentConstraint1.crx" "ear1_R_JNT.rx";
connectAttr "ear1_R_JNT_parentConstraint1.cry" "ear1_R_JNT.ry";
connectAttr "ear1_R_JNT_parentConstraint1.crz" "ear1_R_JNT.rz";
connectAttr "ear1_R_JNT.s" "ear2_R_JNT.is";
connectAttr "ear2_R_JNT_parentConstraint1.ctx" "ear2_R_JNT.tx";
connectAttr "ear2_R_JNT_parentConstraint1.cty" "ear2_R_JNT.ty";
connectAttr "ear2_R_JNT_parentConstraint1.ctz" "ear2_R_JNT.tz";
connectAttr "ear2_R_JNT_parentConstraint1.crx" "ear2_R_JNT.rx";
connectAttr "ear2_R_JNT_parentConstraint1.cry" "ear2_R_JNT.ry";
connectAttr "ear2_R_JNT_parentConstraint1.crz" "ear2_R_JNT.rz";
connectAttr "ear2_R_JNT.s" "|root_CTRL|JNT_GRP|pelvis_JNT|chest_JNT|head_JNT|ear1_R_JNT|ear2_R_JNT|earTIP_JNT.is"
		;
connectAttr "ear2_R_JNT.ro" "ear2_R_JNT_parentConstraint1.cro";
connectAttr "ear2_R_JNT.pim" "ear2_R_JNT_parentConstraint1.cpim";
connectAttr "ear2_R_JNT.rp" "ear2_R_JNT_parentConstraint1.crp";
connectAttr "ear2_R_JNT.rpt" "ear2_R_JNT_parentConstraint1.crt";
connectAttr "ear2_R_JNT.jo" "ear2_R_JNT_parentConstraint1.cjo";
connectAttr "ear2_R_CTRL.t" "ear2_R_JNT_parentConstraint1.tg[0].tt";
connectAttr "ear2_R_CTRL.rp" "ear2_R_JNT_parentConstraint1.tg[0].trp";
connectAttr "ear2_R_CTRL.rpt" "ear2_R_JNT_parentConstraint1.tg[0].trt";
connectAttr "ear2_R_CTRL.r" "ear2_R_JNT_parentConstraint1.tg[0].tr";
connectAttr "ear2_R_CTRL.ro" "ear2_R_JNT_parentConstraint1.tg[0].tro";
connectAttr "ear2_R_CTRL.s" "ear2_R_JNT_parentConstraint1.tg[0].ts";
connectAttr "ear2_R_CTRL.pm" "ear2_R_JNT_parentConstraint1.tg[0].tpm";
connectAttr "ear2_R_JNT_parentConstraint1.w0" "ear2_R_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "ear1_R_JNT.ro" "ear1_R_JNT_parentConstraint1.cro";
connectAttr "ear1_R_JNT.pim" "ear1_R_JNT_parentConstraint1.cpim";
connectAttr "ear1_R_JNT.rp" "ear1_R_JNT_parentConstraint1.crp";
connectAttr "ear1_R_JNT.rpt" "ear1_R_JNT_parentConstraint1.crt";
connectAttr "ear1_R_JNT.jo" "ear1_R_JNT_parentConstraint1.cjo";
connectAttr "ear1_R_CTRL.t" "ear1_R_JNT_parentConstraint1.tg[0].tt";
connectAttr "ear1_R_CTRL.rp" "ear1_R_JNT_parentConstraint1.tg[0].trp";
connectAttr "ear1_R_CTRL.rpt" "ear1_R_JNT_parentConstraint1.tg[0].trt";
connectAttr "ear1_R_CTRL.r" "ear1_R_JNT_parentConstraint1.tg[0].tr";
connectAttr "ear1_R_CTRL.ro" "ear1_R_JNT_parentConstraint1.tg[0].tro";
connectAttr "ear1_R_CTRL.s" "ear1_R_JNT_parentConstraint1.tg[0].ts";
connectAttr "ear1_R_CTRL.pm" "ear1_R_JNT_parentConstraint1.tg[0].tpm";
connectAttr "ear1_R_JNT_parentConstraint1.w0" "ear1_R_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "head_JNT.ro" "head_JNT_parentConstraint1.cro";
connectAttr "head_JNT.pim" "head_JNT_parentConstraint1.cpim";
connectAttr "head_JNT.rp" "head_JNT_parentConstraint1.crp";
connectAttr "head_JNT.rpt" "head_JNT_parentConstraint1.crt";
connectAttr "head_JNT.jo" "head_JNT_parentConstraint1.cjo";
connectAttr "head_CTRL.t" "head_JNT_parentConstraint1.tg[0].tt";
connectAttr "head_CTRL.rp" "head_JNT_parentConstraint1.tg[0].trp";
connectAttr "head_CTRL.rpt" "head_JNT_parentConstraint1.tg[0].trt";
connectAttr "head_CTRL.r" "head_JNT_parentConstraint1.tg[0].tr";
connectAttr "head_CTRL.ro" "head_JNT_parentConstraint1.tg[0].tro";
connectAttr "head_CTRL.s" "head_JNT_parentConstraint1.tg[0].ts";
connectAttr "head_CTRL.pm" "head_JNT_parentConstraint1.tg[0].tpm";
connectAttr "head_JNT_parentConstraint1.w0" "head_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "chest_JNT.s" "shoulder_L_JNT.is";
connectAttr "shoulder_L_JNT_parentConstraint1.ctx" "shoulder_L_JNT.tx";
connectAttr "shoulder_L_JNT_parentConstraint1.cty" "shoulder_L_JNT.ty";
connectAttr "shoulder_L_JNT_parentConstraint1.ctz" "shoulder_L_JNT.tz";
connectAttr "shoulder_L_JNT_parentConstraint1.crx" "shoulder_L_JNT.rx";
connectAttr "shoulder_L_JNT_parentConstraint1.cry" "shoulder_L_JNT.ry";
connectAttr "shoulder_L_JNT_parentConstraint1.crz" "shoulder_L_JNT.rz";
connectAttr "shoulder_L_JNT.s" "elbow_L_JNT.is";
connectAttr "elbow_L_JNT_parentConstraint1.ctx" "elbow_L_JNT.tx";
connectAttr "elbow_L_JNT_parentConstraint1.cty" "elbow_L_JNT.ty";
connectAttr "elbow_L_JNT_parentConstraint1.ctz" "elbow_L_JNT.tz";
connectAttr "elbow_L_JNT_parentConstraint1.crx" "elbow_L_JNT.rx";
connectAttr "elbow_L_JNT_parentConstraint1.cry" "elbow_L_JNT.ry";
connectAttr "elbow_L_JNT_parentConstraint1.crz" "elbow_L_JNT.rz";
connectAttr "elbow_L_JNT.s" "handTIP_L_JNT.is";
connectAttr "elbow_L_JNT.ro" "elbow_L_JNT_parentConstraint1.cro";
connectAttr "elbow_L_JNT.pim" "elbow_L_JNT_parentConstraint1.cpim";
connectAttr "elbow_L_JNT.rp" "elbow_L_JNT_parentConstraint1.crp";
connectAttr "elbow_L_JNT.rpt" "elbow_L_JNT_parentConstraint1.crt";
connectAttr "elbow_L_JNT.jo" "elbow_L_JNT_parentConstraint1.cjo";
connectAttr "elbow_L_CTRL.t" "elbow_L_JNT_parentConstraint1.tg[0].tt";
connectAttr "elbow_L_CTRL.rp" "elbow_L_JNT_parentConstraint1.tg[0].trp";
connectAttr "elbow_L_CTRL.rpt" "elbow_L_JNT_parentConstraint1.tg[0].trt";
connectAttr "elbow_L_CTRL.r" "elbow_L_JNT_parentConstraint1.tg[0].tr";
connectAttr "elbow_L_CTRL.ro" "elbow_L_JNT_parentConstraint1.tg[0].tro";
connectAttr "elbow_L_CTRL.s" "elbow_L_JNT_parentConstraint1.tg[0].ts";
connectAttr "elbow_L_CTRL.pm" "elbow_L_JNT_parentConstraint1.tg[0].tpm";
connectAttr "elbow_L_JNT_parentConstraint1.w0" "elbow_L_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "shoulder_L_JNT.ro" "shoulder_L_JNT_parentConstraint1.cro";
connectAttr "shoulder_L_JNT.pim" "shoulder_L_JNT_parentConstraint1.cpim";
connectAttr "shoulder_L_JNT.rp" "shoulder_L_JNT_parentConstraint1.crp";
connectAttr "shoulder_L_JNT.rpt" "shoulder_L_JNT_parentConstraint1.crt";
connectAttr "shoulder_L_JNT.jo" "shoulder_L_JNT_parentConstraint1.cjo";
connectAttr "shoulder_L_CTRL.t" "shoulder_L_JNT_parentConstraint1.tg[0].tt";
connectAttr "shoulder_L_CTRL.rp" "shoulder_L_JNT_parentConstraint1.tg[0].trp";
connectAttr "shoulder_L_CTRL.rpt" "shoulder_L_JNT_parentConstraint1.tg[0].trt";
connectAttr "shoulder_L_CTRL.r" "shoulder_L_JNT_parentConstraint1.tg[0].tr";
connectAttr "shoulder_L_CTRL.ro" "shoulder_L_JNT_parentConstraint1.tg[0].tro";
connectAttr "shoulder_L_CTRL.s" "shoulder_L_JNT_parentConstraint1.tg[0].ts";
connectAttr "shoulder_L_CTRL.pm" "shoulder_L_JNT_parentConstraint1.tg[0].tpm";
connectAttr "shoulder_L_JNT_parentConstraint1.w0" "shoulder_L_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "chest_JNT.s" "shoulder_R_JNT.is";
connectAttr "shoulder_R_JNT_parentConstraint1.ctx" "shoulder_R_JNT.tx";
connectAttr "shoulder_R_JNT_parentConstraint1.cty" "shoulder_R_JNT.ty";
connectAttr "shoulder_R_JNT_parentConstraint1.ctz" "shoulder_R_JNT.tz";
connectAttr "shoulder_R_JNT_parentConstraint1.crx" "shoulder_R_JNT.rx";
connectAttr "shoulder_R_JNT_parentConstraint1.cry" "shoulder_R_JNT.ry";
connectAttr "shoulder_R_JNT_parentConstraint1.crz" "shoulder_R_JNT.rz";
connectAttr "shoulder_R_JNT.s" "elbow_R_JNT.is";
connectAttr "elbow_R_JNT_parentConstraint1.ctx" "elbow_R_JNT.tx";
connectAttr "elbow_R_JNT_parentConstraint1.cty" "elbow_R_JNT.ty";
connectAttr "elbow_R_JNT_parentConstraint1.ctz" "elbow_R_JNT.tz";
connectAttr "elbow_R_JNT_parentConstraint1.crx" "elbow_R_JNT.rx";
connectAttr "elbow_R_JNT_parentConstraint1.cry" "elbow_R_JNT.ry";
connectAttr "elbow_R_JNT_parentConstraint1.crz" "elbow_R_JNT.rz";
connectAttr "elbow_R_JNT.s" "handTIP_R_JNT.is";
connectAttr "elbow_R_JNT.ro" "elbow_R_JNT_parentConstraint1.cro";
connectAttr "elbow_R_JNT.pim" "elbow_R_JNT_parentConstraint1.cpim";
connectAttr "elbow_R_JNT.rp" "elbow_R_JNT_parentConstraint1.crp";
connectAttr "elbow_R_JNT.rpt" "elbow_R_JNT_parentConstraint1.crt";
connectAttr "elbow_R_JNT.jo" "elbow_R_JNT_parentConstraint1.cjo";
connectAttr "elbow_R_CTRL.t" "elbow_R_JNT_parentConstraint1.tg[0].tt";
connectAttr "elbow_R_CTRL.rp" "elbow_R_JNT_parentConstraint1.tg[0].trp";
connectAttr "elbow_R_CTRL.rpt" "elbow_R_JNT_parentConstraint1.tg[0].trt";
connectAttr "elbow_R_CTRL.r" "elbow_R_JNT_parentConstraint1.tg[0].tr";
connectAttr "elbow_R_CTRL.ro" "elbow_R_JNT_parentConstraint1.tg[0].tro";
connectAttr "elbow_R_CTRL.s" "elbow_R_JNT_parentConstraint1.tg[0].ts";
connectAttr "elbow_R_CTRL.pm" "elbow_R_JNT_parentConstraint1.tg[0].tpm";
connectAttr "elbow_R_JNT_parentConstraint1.w0" "elbow_R_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "shoulder_R_JNT.ro" "shoulder_R_JNT_parentConstraint1.cro";
connectAttr "shoulder_R_JNT.pim" "shoulder_R_JNT_parentConstraint1.cpim";
connectAttr "shoulder_R_JNT.rp" "shoulder_R_JNT_parentConstraint1.crp";
connectAttr "shoulder_R_JNT.rpt" "shoulder_R_JNT_parentConstraint1.crt";
connectAttr "shoulder_R_JNT.jo" "shoulder_R_JNT_parentConstraint1.cjo";
connectAttr "shoulder_R_CTRL.t" "shoulder_R_JNT_parentConstraint1.tg[0].tt";
connectAttr "shoulder_R_CTRL.rp" "shoulder_R_JNT_parentConstraint1.tg[0].trp";
connectAttr "shoulder_R_CTRL.rpt" "shoulder_R_JNT_parentConstraint1.tg[0].trt";
connectAttr "shoulder_R_CTRL.r" "shoulder_R_JNT_parentConstraint1.tg[0].tr";
connectAttr "shoulder_R_CTRL.ro" "shoulder_R_JNT_parentConstraint1.tg[0].tro";
connectAttr "shoulder_R_CTRL.s" "shoulder_R_JNT_parentConstraint1.tg[0].ts";
connectAttr "shoulder_R_CTRL.pm" "shoulder_R_JNT_parentConstraint1.tg[0].tpm";
connectAttr "shoulder_R_JNT_parentConstraint1.w0" "shoulder_R_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "chest_JNT.ro" "chest_JNT_parentConstraint1.cro";
connectAttr "chest_JNT.pim" "chest_JNT_parentConstraint1.cpim";
connectAttr "chest_JNT.rp" "chest_JNT_parentConstraint1.crp";
connectAttr "chest_JNT.rpt" "chest_JNT_parentConstraint1.crt";
connectAttr "chest_JNT.jo" "chest_JNT_parentConstraint1.cjo";
connectAttr "chest_CTRL.t" "chest_JNT_parentConstraint1.tg[0].tt";
connectAttr "chest_CTRL.rp" "chest_JNT_parentConstraint1.tg[0].trp";
connectAttr "chest_CTRL.rpt" "chest_JNT_parentConstraint1.tg[0].trt";
connectAttr "chest_CTRL.r" "chest_JNT_parentConstraint1.tg[0].tr";
connectAttr "chest_CTRL.ro" "chest_JNT_parentConstraint1.tg[0].tro";
connectAttr "chest_CTRL.s" "chest_JNT_parentConstraint1.tg[0].ts";
connectAttr "chest_CTRL.pm" "chest_JNT_parentConstraint1.tg[0].tpm";
connectAttr "chest_JNT_parentConstraint1.w0" "chest_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "pelvis_JNT.s" "thigh_L_JNT.is";
connectAttr "thigh_L_JNT_parentConstraint1.ctx" "thigh_L_JNT.tx";
connectAttr "thigh_L_JNT_parentConstraint1.cty" "thigh_L_JNT.ty";
connectAttr "thigh_L_JNT_parentConstraint1.ctz" "thigh_L_JNT.tz";
connectAttr "thigh_L_JNT_parentConstraint1.crx" "thigh_L_JNT.rx";
connectAttr "thigh_L_JNT_parentConstraint1.cry" "thigh_L_JNT.ry";
connectAttr "thigh_L_JNT_parentConstraint1.crz" "thigh_L_JNT.rz";
connectAttr "thigh_L_JNT.s" "foot_L_JNT.is";
connectAttr "foot_L_JNT_parentConstraint1.ctx" "foot_L_JNT.tx";
connectAttr "foot_L_JNT_parentConstraint1.cty" "foot_L_JNT.ty";
connectAttr "foot_L_JNT_parentConstraint1.ctz" "foot_L_JNT.tz";
connectAttr "foot_L_JNT_parentConstraint1.crx" "foot_L_JNT.rx";
connectAttr "foot_L_JNT_parentConstraint1.cry" "foot_L_JNT.ry";
connectAttr "foot_L_JNT_parentConstraint1.crz" "foot_L_JNT.rz";
connectAttr "foot_L_JNT.s" "footTIP_L_JNT.is";
connectAttr "foot_L_JNT.ro" "foot_L_JNT_parentConstraint1.cro";
connectAttr "foot_L_JNT.pim" "foot_L_JNT_parentConstraint1.cpim";
connectAttr "foot_L_JNT.rp" "foot_L_JNT_parentConstraint1.crp";
connectAttr "foot_L_JNT.rpt" "foot_L_JNT_parentConstraint1.crt";
connectAttr "foot_L_JNT.jo" "foot_L_JNT_parentConstraint1.cjo";
connectAttr "foot_L_CTRL.t" "foot_L_JNT_parentConstraint1.tg[0].tt";
connectAttr "foot_L_CTRL.rp" "foot_L_JNT_parentConstraint1.tg[0].trp";
connectAttr "foot_L_CTRL.rpt" "foot_L_JNT_parentConstraint1.tg[0].trt";
connectAttr "foot_L_CTRL.r" "foot_L_JNT_parentConstraint1.tg[0].tr";
connectAttr "foot_L_CTRL.ro" "foot_L_JNT_parentConstraint1.tg[0].tro";
connectAttr "foot_L_CTRL.s" "foot_L_JNT_parentConstraint1.tg[0].ts";
connectAttr "foot_L_CTRL.pm" "foot_L_JNT_parentConstraint1.tg[0].tpm";
connectAttr "foot_L_JNT_parentConstraint1.w0" "foot_L_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "thigh_L_JNT.ro" "thigh_L_JNT_parentConstraint1.cro";
connectAttr "thigh_L_JNT.pim" "thigh_L_JNT_parentConstraint1.cpim";
connectAttr "thigh_L_JNT.rp" "thigh_L_JNT_parentConstraint1.crp";
connectAttr "thigh_L_JNT.rpt" "thigh_L_JNT_parentConstraint1.crt";
connectAttr "thigh_L_JNT.jo" "thigh_L_JNT_parentConstraint1.cjo";
connectAttr "leg_L_CTRL.t" "thigh_L_JNT_parentConstraint1.tg[0].tt";
connectAttr "leg_L_CTRL.rp" "thigh_L_JNT_parentConstraint1.tg[0].trp";
connectAttr "leg_L_CTRL.rpt" "thigh_L_JNT_parentConstraint1.tg[0].trt";
connectAttr "leg_L_CTRL.r" "thigh_L_JNT_parentConstraint1.tg[0].tr";
connectAttr "leg_L_CTRL.ro" "thigh_L_JNT_parentConstraint1.tg[0].tro";
connectAttr "leg_L_CTRL.s" "thigh_L_JNT_parentConstraint1.tg[0].ts";
connectAttr "leg_L_CTRL.pm" "thigh_L_JNT_parentConstraint1.tg[0].tpm";
connectAttr "thigh_L_JNT_parentConstraint1.w0" "thigh_L_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "pelvis_JNT.s" "thigh_R_JNT.is";
connectAttr "thigh_R_JNT_parentConstraint1.ctx" "thigh_R_JNT.tx";
connectAttr "thigh_R_JNT_parentConstraint1.cty" "thigh_R_JNT.ty";
connectAttr "thigh_R_JNT_parentConstraint1.ctz" "thigh_R_JNT.tz";
connectAttr "thigh_R_JNT_parentConstraint1.crx" "thigh_R_JNT.rx";
connectAttr "thigh_R_JNT_parentConstraint1.cry" "thigh_R_JNT.ry";
connectAttr "thigh_R_JNT_parentConstraint1.crz" "thigh_R_JNT.rz";
connectAttr "thigh_R_JNT.s" "foot_R_JNT.is";
connectAttr "foot_R_JNT_parentConstraint1.ctx" "foot_R_JNT.tx";
connectAttr "foot_R_JNT_parentConstraint1.cty" "foot_R_JNT.ty";
connectAttr "foot_R_JNT_parentConstraint1.ctz" "foot_R_JNT.tz";
connectAttr "foot_R_JNT_parentConstraint1.crx" "foot_R_JNT.rx";
connectAttr "foot_R_JNT_parentConstraint1.cry" "foot_R_JNT.ry";
connectAttr "foot_R_JNT_parentConstraint1.crz" "foot_R_JNT.rz";
connectAttr "foot_R_JNT.s" "footTIP_R_JNT.is";
connectAttr "foot_R_JNT.ro" "foot_R_JNT_parentConstraint1.cro";
connectAttr "foot_R_JNT.pim" "foot_R_JNT_parentConstraint1.cpim";
connectAttr "foot_R_JNT.rp" "foot_R_JNT_parentConstraint1.crp";
connectAttr "foot_R_JNT.rpt" "foot_R_JNT_parentConstraint1.crt";
connectAttr "foot_R_JNT.jo" "foot_R_JNT_parentConstraint1.cjo";
connectAttr "foot_R_CTRL.t" "foot_R_JNT_parentConstraint1.tg[0].tt";
connectAttr "foot_R_CTRL.rp" "foot_R_JNT_parentConstraint1.tg[0].trp";
connectAttr "foot_R_CTRL.rpt" "foot_R_JNT_parentConstraint1.tg[0].trt";
connectAttr "foot_R_CTRL.r" "foot_R_JNT_parentConstraint1.tg[0].tr";
connectAttr "foot_R_CTRL.ro" "foot_R_JNT_parentConstraint1.tg[0].tro";
connectAttr "foot_R_CTRL.s" "foot_R_JNT_parentConstraint1.tg[0].ts";
connectAttr "foot_R_CTRL.pm" "foot_R_JNT_parentConstraint1.tg[0].tpm";
connectAttr "foot_R_JNT_parentConstraint1.w0" "foot_R_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "thigh_R_JNT.ro" "thigh_R_JNT_parentConstraint1.cro";
connectAttr "thigh_R_JNT.pim" "thigh_R_JNT_parentConstraint1.cpim";
connectAttr "thigh_R_JNT.rp" "thigh_R_JNT_parentConstraint1.crp";
connectAttr "thigh_R_JNT.rpt" "thigh_R_JNT_parentConstraint1.crt";
connectAttr "thigh_R_JNT.jo" "thigh_R_JNT_parentConstraint1.cjo";
connectAttr "leg_R_CTRL.t" "thigh_R_JNT_parentConstraint1.tg[0].tt";
connectAttr "leg_R_CTRL.rp" "thigh_R_JNT_parentConstraint1.tg[0].trp";
connectAttr "leg_R_CTRL.rpt" "thigh_R_JNT_parentConstraint1.tg[0].trt";
connectAttr "leg_R_CTRL.r" "thigh_R_JNT_parentConstraint1.tg[0].tr";
connectAttr "leg_R_CTRL.ro" "thigh_R_JNT_parentConstraint1.tg[0].tro";
connectAttr "leg_R_CTRL.s" "thigh_R_JNT_parentConstraint1.tg[0].ts";
connectAttr "leg_R_CTRL.pm" "thigh_R_JNT_parentConstraint1.tg[0].tpm";
connectAttr "thigh_R_JNT_parentConstraint1.w0" "thigh_R_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "pelvis_JNT.ro" "pelvis_JNT_parentConstraint1.cro";
connectAttr "pelvis_JNT.pim" "pelvis_JNT_parentConstraint1.cpim";
connectAttr "pelvis_JNT.rp" "pelvis_JNT_parentConstraint1.crp";
connectAttr "pelvis_JNT.rpt" "pelvis_JNT_parentConstraint1.crt";
connectAttr "pelvis_JNT.jo" "pelvis_JNT_parentConstraint1.cjo";
connectAttr "hip_CTRL.t" "pelvis_JNT_parentConstraint1.tg[0].tt";
connectAttr "hip_CTRL.rp" "pelvis_JNT_parentConstraint1.tg[0].trp";
connectAttr "hip_CTRL.rpt" "pelvis_JNT_parentConstraint1.tg[0].trt";
connectAttr "hip_CTRL.r" "pelvis_JNT_parentConstraint1.tg[0].tr";
connectAttr "hip_CTRL.ro" "pelvis_JNT_parentConstraint1.tg[0].tro";
connectAttr "hip_CTRL.s" "pelvis_JNT_parentConstraint1.tg[0].ts";
connectAttr "hip_CTRL.pm" "pelvis_JNT_parentConstraint1.tg[0].tpm";
connectAttr "pelvis_JNT_parentConstraint1.w0" "pelvis_JNT_parentConstraint1.tg[0].tw"
		;
connectAttr "GEO_LYR.di" "GEO_GRP.do";
connectAttr "groupId56.id" "rabbit_GEOShape.iog.og[1].gid";
connectAttr "textureEditorIsolateSelectSet.mwc" "rabbit_GEOShape.iog.og[1].gco";
connectAttr "skinCluster1.og[0]" "rabbit_GEOShape.i";
relationship "link" ":lightLinker1" ":initialShadingGroup.message" ":defaultLightSet.message";
relationship "link" ":lightLinker1" ":initialParticleSE.message" ":defaultLightSet.message";
relationship "link" ":lightLinker1" "typeBlinnSG.message" ":defaultLightSet.message";
relationship "link" ":lightLinker1" "surfaceShader1SG.message" ":defaultLightSet.message";
relationship "shadowLink" ":lightLinker1" ":initialShadingGroup.message" ":defaultLightSet.message";
relationship "shadowLink" ":lightLinker1" ":initialParticleSE.message" ":defaultLightSet.message";
relationship "shadowLink" ":lightLinker1" "typeBlinnSG.message" ":defaultLightSet.message";
relationship "shadowLink" ":lightLinker1" "surfaceShader1SG.message" ":defaultLightSet.message";
connectAttr "layerManager.dli[0]" "defaultLayer.id";
connectAttr "renderLayerManager.rlmi[0]" "defaultRenderLayer.rlid";
connectAttr "groupId56.msg" "textureEditorIsolateSelectSet.gn" -na;
connectAttr "rabbit_GEOShape.iog.og[1]" "textureEditorIsolateSelectSet.dsm" -na;
connectAttr "typeBlinn.oc" "typeBlinnSG.ss";
connectAttr "typeBlinnSG.msg" "materialInfo1.sg";
connectAttr "typeBlinn.msg" "materialInfo1.m";
connectAttr "file1.oc" "surfaceShader1.oc";
connectAttr "surfaceShader1.oc" "surfaceShader1SG.ss";
connectAttr "rabbit_GEOShape.iog" "surfaceShader1SG.dsm" -na;
connectAttr "surfaceShader1SG.msg" "materialInfo2.sg";
connectAttr "surfaceShader1.msg" "materialInfo2.m";
connectAttr "file1.msg" "materialInfo2.t" -na;
connectAttr ":defaultColorMgtGlobals.cme" "file1.cme";
connectAttr ":defaultColorMgtGlobals.cfe" "file1.cmcf";
connectAttr ":defaultColorMgtGlobals.cfp" "file1.cmcp";
connectAttr ":defaultColorMgtGlobals.wsn" "file1.ws";
connectAttr "place2dTexture1.c" "file1.c";
connectAttr "place2dTexture1.tf" "file1.tf";
connectAttr "place2dTexture1.rf" "file1.rf";
connectAttr "place2dTexture1.mu" "file1.mu";
connectAttr "place2dTexture1.mv" "file1.mv";
connectAttr "place2dTexture1.s" "file1.s";
connectAttr "place2dTexture1.wu" "file1.wu";
connectAttr "place2dTexture1.wv" "file1.wv";
connectAttr "place2dTexture1.re" "file1.re";
connectAttr "place2dTexture1.of" "file1.of";
connectAttr "place2dTexture1.r" "file1.ro";
connectAttr "place2dTexture1.n" "file1.n";
connectAttr "place2dTexture1.vt1" "file1.vt1";
connectAttr "place2dTexture1.vt2" "file1.vt2";
connectAttr "place2dTexture1.vt3" "file1.vt3";
connectAttr "place2dTexture1.vc1" "file1.vc1";
connectAttr "place2dTexture1.o" "file1.uv";
connectAttr "place2dTexture1.ofs" "file1.fs";
connectAttr "skinCluster1GroupParts.og" "skinCluster1.ip[0].ig";
connectAttr "rabbit_GEOShapeOrig.o" "skinCluster1.orggeom[0]";
connectAttr "bindPose1.msg" "skinCluster1.bp";
connectAttr "pelvis_JNT.wm" "skinCluster1.ma[0]";
connectAttr "chest_JNT.wm" "skinCluster1.ma[1]";
connectAttr "head_JNT.wm" "skinCluster1.ma[2]";
connectAttr "shoulder_L_JNT.wm" "skinCluster1.ma[10]";
connectAttr "elbow_L_JNT.wm" "skinCluster1.ma[11]";
connectAttr "shoulder_R_JNT.wm" "skinCluster1.ma[13]";
connectAttr "elbow_R_JNT.wm" "skinCluster1.ma[14]";
connectAttr "thigh_L_JNT.wm" "skinCluster1.ma[16]";
connectAttr "foot_L_JNT.wm" "skinCluster1.ma[17]";
connectAttr "thigh_R_JNT.wm" "skinCluster1.ma[19]";
connectAttr "foot_R_JNT.wm" "skinCluster1.ma[20]";
connectAttr "ear1_L_JNT.wm" "skinCluster1.ma[22]";
connectAttr "ear2_L_JNT.wm" "skinCluster1.ma[23]";
connectAttr "ear1_R_JNT.wm" "skinCluster1.ma[25]";
connectAttr "ear2_R_JNT.wm" "skinCluster1.ma[26]";
connectAttr "pelvis_JNT.liw" "skinCluster1.lw[0]";
connectAttr "chest_JNT.liw" "skinCluster1.lw[1]";
connectAttr "head_JNT.liw" "skinCluster1.lw[2]";
connectAttr "shoulder_L_JNT.liw" "skinCluster1.lw[10]";
connectAttr "elbow_L_JNT.liw" "skinCluster1.lw[11]";
connectAttr "shoulder_R_JNT.liw" "skinCluster1.lw[13]";
connectAttr "elbow_R_JNT.liw" "skinCluster1.lw[14]";
connectAttr "thigh_L_JNT.liw" "skinCluster1.lw[16]";
connectAttr "foot_L_JNT.liw" "skinCluster1.lw[17]";
connectAttr "thigh_R_JNT.liw" "skinCluster1.lw[19]";
connectAttr "foot_R_JNT.liw" "skinCluster1.lw[20]";
connectAttr "ear1_L_JNT.liw" "skinCluster1.lw[22]";
connectAttr "ear2_L_JNT.liw" "skinCluster1.lw[23]";
connectAttr "ear1_R_JNT.liw" "skinCluster1.lw[25]";
connectAttr "ear2_R_JNT.liw" "skinCluster1.lw[26]";
connectAttr "pelvis_JNT.obcc" "skinCluster1.ifcl[0]";
connectAttr "chest_JNT.obcc" "skinCluster1.ifcl[1]";
connectAttr "head_JNT.obcc" "skinCluster1.ifcl[2]";
connectAttr "shoulder_L_JNT.obcc" "skinCluster1.ifcl[10]";
connectAttr "elbow_L_JNT.obcc" "skinCluster1.ifcl[11]";
connectAttr "shoulder_R_JNT.obcc" "skinCluster1.ifcl[13]";
connectAttr "elbow_R_JNT.obcc" "skinCluster1.ifcl[14]";
connectAttr "thigh_L_JNT.obcc" "skinCluster1.ifcl[16]";
connectAttr "foot_L_JNT.obcc" "skinCluster1.ifcl[17]";
connectAttr "thigh_R_JNT.obcc" "skinCluster1.ifcl[19]";
connectAttr "foot_R_JNT.obcc" "skinCluster1.ifcl[20]";
connectAttr "ear1_L_JNT.obcc" "skinCluster1.ifcl[22]";
connectAttr "ear2_L_JNT.obcc" "skinCluster1.ifcl[23]";
connectAttr "ear1_R_JNT.obcc" "skinCluster1.ifcl[25]";
connectAttr "ear2_R_JNT.obcc" "skinCluster1.ifcl[26]";
connectAttr "shoulder_R_JNT.msg" "skinCluster1.ptt";
connectAttr "rabbit_GEOShapeOrig.w" "skinCluster1GroupParts.ig";
connectAttr "groupId56.id" "skinCluster1GroupParts.gi";
connectAttr "pelvis_JNT.msg" "bindPose1.m[0]";
connectAttr "chest_JNT.msg" "bindPose1.m[1]";
connectAttr "head_JNT.msg" "bindPose1.m[2]";
connectAttr "shoulder_L_JNT.msg" "bindPose1.m[10]";
connectAttr "elbow_L_JNT.msg" "bindPose1.m[11]";
connectAttr "shoulder_R_JNT.msg" "bindPose1.m[13]";
connectAttr "elbow_R_JNT.msg" "bindPose1.m[14]";
connectAttr "thigh_L_JNT.msg" "bindPose1.m[16]";
connectAttr "foot_L_JNT.msg" "bindPose1.m[17]";
connectAttr "thigh_R_JNT.msg" "bindPose1.m[19]";
connectAttr "foot_R_JNT.msg" "bindPose1.m[20]";
connectAttr "JNT_GRP.msg" "bindPose1.m[22]";
connectAttr "ear1_L_JNT.msg" "bindPose1.m[23]";
connectAttr "ear2_L_JNT.msg" "bindPose1.m[24]";
connectAttr "ear1_R_JNT.msg" "bindPose1.m[26]";
connectAttr "ear2_R_JNT.msg" "bindPose1.m[27]";
connectAttr "bindPose1.m[22]" "bindPose1.p[0]";
connectAttr "bindPose1.m[0]" "bindPose1.p[1]";
connectAttr "bindPose1.m[1]" "bindPose1.p[2]";
connectAttr "bindPose1.m[2]" "bindPose1.p[4]";
connectAttr "bindPose1.m[4]" "bindPose1.p[5]";
connectAttr "bindPose1.m[2]" "bindPose1.p[7]";
connectAttr "bindPose1.m[7]" "bindPose1.p[8]";
connectAttr "bindPose1.m[1]" "bindPose1.p[10]";
connectAttr "bindPose1.m[10]" "bindPose1.p[11]";
connectAttr "bindPose1.m[1]" "bindPose1.p[13]";
connectAttr "bindPose1.m[13]" "bindPose1.p[14]";
connectAttr "bindPose1.m[0]" "bindPose1.p[16]";
connectAttr "bindPose1.m[16]" "bindPose1.p[17]";
connectAttr "bindPose1.m[0]" "bindPose1.p[19]";
connectAttr "bindPose1.m[19]" "bindPose1.p[20]";
connectAttr "bindPose1.w" "bindPose1.p[22]";
connectAttr "bindPose1.m[2]" "bindPose1.p[23]";
connectAttr "bindPose1.m[23]" "bindPose1.p[24]";
connectAttr "bindPose1.m[2]" "bindPose1.p[26]";
connectAttr "bindPose1.m[26]" "bindPose1.p[27]";
connectAttr "pelvis_JNT.bps" "bindPose1.wm[0]";
connectAttr "chest_JNT.bps" "bindPose1.wm[1]";
connectAttr "head_JNT.bps" "bindPose1.wm[2]";
connectAttr "shoulder_L_JNT.bps" "bindPose1.wm[10]";
connectAttr "elbow_L_JNT.bps" "bindPose1.wm[11]";
connectAttr "shoulder_R_JNT.bps" "bindPose1.wm[13]";
connectAttr "elbow_R_JNT.bps" "bindPose1.wm[14]";
connectAttr "thigh_L_JNT.bps" "bindPose1.wm[16]";
connectAttr "foot_L_JNT.bps" "bindPose1.wm[17]";
connectAttr "thigh_R_JNT.bps" "bindPose1.wm[19]";
connectAttr "foot_R_JNT.bps" "bindPose1.wm[20]";
connectAttr "ear1_L_JNT.bps" "bindPose1.wm[23]";
connectAttr "ear2_L_JNT.bps" "bindPose1.wm[24]";
connectAttr "ear1_R_JNT.bps" "bindPose1.wm[26]";
connectAttr "ear2_R_JNT.bps" "bindPose1.wm[27]";
connectAttr "layerManager.dli[1]" "GEO_LYR.id";
connectAttr "layerManager.dli[2]" "JNT_LYR.id";
connectAttr "layerManager.dli[3]" "CTRL_LYR.id";
connectAttr "makeNurbCircle1.oc" "transformGeometry1.ig";
connectAttr "makeNurbCircle3.oc" "transformGeometry2.ig";
connectAttr "surfaceShader1.msg" "hyperShadePrimaryNodeEditorSavedTabsInfo.tgi[0].ni[0].dn"
		;
connectAttr "file1.msg" "hyperShadePrimaryNodeEditorSavedTabsInfo.tgi[0].ni[1].dn"
		;
connectAttr "surfaceShader1SG.msg" "hyperShadePrimaryNodeEditorSavedTabsInfo.tgi[0].ni[2].dn"
		;
connectAttr "place2dTexture1.msg" "hyperShadePrimaryNodeEditorSavedTabsInfo.tgi[0].ni[3].dn"
		;
connectAttr "typeBlinnSG.pa" ":renderPartition.st" -na;
connectAttr "surfaceShader1SG.pa" ":renderPartition.st" -na;
connectAttr "typeBlinn.msg" ":defaultShaderList1.s" -na;
connectAttr "surfaceShader1.msg" ":defaultShaderList1.s" -na;
connectAttr "place2dTexture1.msg" ":defaultRenderUtilityList1.u" -na;
connectAttr "defaultRenderLayer.msg" ":defaultRenderingList1.r" -na;
connectAttr "file1.msg" ":defaultTextureList1.tx" -na;
// End of rabbitModel.015.ma
