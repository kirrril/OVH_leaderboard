<?php
header("Access-Control-Allow-Origin: *");
header("Content-Type: text/plain");
error_reporting(0);
ini_set('display_errors', 0);

$mysqli = new mysqli ('localhost', 'leaderuser', 'Lopat@_123455432', 'leaderboard');

if ($mysqli->connect_error)
{
        echo "ERROR";
        exit;
}

$pseudo = $_GET['pseudo'] ?? '';
$new_score = (int)$_GET['score'] ?? 0;

if (empty($pseudo) || strlen($pseudo) < 2 || $new_score < 0)
{
    echo "INVALID";
    exit;
}

$pseudo = $mysqli->real_escape_string($pseudo);

$result = $mysqli->query("SELECT score FROM scores WHERE player_name = '$pseudo'");

if ($result->num_rows == 0)
{
    echo "PSEUDONOTFOUND";
    exit;
}

$old_score = $result->fetch_assoc()['score'];

if ($new_score > $old_score)
{
    $mysqli->query("UPDATE scores SET score = $new_score WHERE player_name = '$pseudo'");
    echo "SCOREUPDATED";
}
else
{
    echo "SCORELOWER";
}

$mysqli->close();
?>