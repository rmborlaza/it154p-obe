<?php 
include_once('../connects.php');

$paramsSet = isset($_POST['username']) && isset($_POST['password']) && isset($_POST['first_name']) && isset($_POST['last_name']) && isset($_POST['account_type']);

if (!$paramsSet)
{
    http_response_code(401);
    echo 'ERROR';
    die();
}

$username = $_POST['username'];
$password = $_POST['password'];
$firstName = $_POST['first_name'];
$lastName = $_POST['last_name'];
$accountType = $_POST['account_type'];
$type = 0;

if ($accountType == 'system') {
    $type = 1;
}

$query = "INSERT INTO user_accounts (username, password, first_name, last_name, is_system_account) VALUES (?, SHA2(?, 256), ?, ?, ?)";
$statement = $con->prepare($query);
$statement->bind_param('ssssi', $username, $password, $firstName, $lastName, $type);
$statement->execute();
echo 'OK';

try {
    
}
catch (Exception $ex) {
    http_response_code(500);
    echo 'ERROR';
}

?>