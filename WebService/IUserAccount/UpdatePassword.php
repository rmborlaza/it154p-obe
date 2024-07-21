<?php 
include_once('../connects.php');

$paramsSet = isset($_POST['user_id']) && isset($_POST['new_password']);
$currentPassSet = isset($_POST['current_password']);

if (!$paramsSet)
{
    http_response_code(401);
    echo 'ERROR';
    die();
}

if ($currentPassSet) {
    $id = $_POST['user_id'];
    $newPass = $_POST['new_password'];
    $oldPass = $_POST['current_password'];

    // Verify Password
    $query = "SELECT * FROM user_accounts WHERE user_id=? AND password=SHA2(?,256)";
    
    $statement = $con->prepare($query);
    $statement->bind_param('is', $id, $oldPass);
    $statement->execute();

    $result = $statement->get_result();
    $numRows = mysqli_num_rows($result);

    if ($numRows > 0) {
        // Change Password
        $changePassQuery = "UPDATE user_accounts SET password=SHA2(?,256) WHERE user_id = ?";

        $stmt = $con->prepare($changePassQuery);
        $stmt->bind_param('si', $newPass, $id);
        $stmt->execute();
        echo 'OK';
    }
    else {
        http_response_code(400);
        echo 'ERROR';
    }
}
else {
    $id = $_POST['user_id'];
    $newPass = $_POST['new_password'];

    $changePassQuery = "UPDATE user_accounts SET password=SHA2(?,256) WHERE user_id = ?";

    $stmt = $con->prepare($changePassQuery);
    $stmt->bind_param('si', $newPass, $id);
    $stmt->execute();
    echo 'OK';
}

?>