<?php
include_once('../connects.php');

$paramsSet = isset($_POST['username']) && isset($_POST['password']);

if (!$paramsSet)
{
    http_response_code(400);
    echo 'ERROR';
    die();
}

header('Content-Type: application/json; charset=utf-8');

$username = $_POST['username'];
$password = $_POST['password'];

$query = "SELECT user_id, username, first_name, last_name, CONVERT_TZ(registration, '+00:00', '+08:00') as registration, is_system_account FROM user_accounts WHERE username=? AND password=SHA2(?,256)";

$statement = $con->prepare($query);
$statement->bind_param('ss', $username, $password);
$statement->execute();

$result = $statement->get_result();
$numRows = mysqli_num_rows($result);

if ($numRows > 0) {
    $user = $result->fetch_assoc();
    $type = 'user';
    if ($user['is_system_account']) {
        $type = 'system';
    }

    $userJson = array('user_id'=>$user['user_id'], 'first_name'=>$user['first_name'], 'last_name'=>$user['last_name'], 'username'=>$user['username'], 'registration'=>$user['registration'], 'account_type'=>$type);
    echo json_encode($userJson);
}
else
{
    http_response_code(400);
    echo 'ERROR';
}
?>