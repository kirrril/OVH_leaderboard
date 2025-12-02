<?php
header("Access-Control-Allow-Origin: *");
header("Content-Type: text/plain");
error_reporting(0);
ini_set('display_errors', 0);

$mysqli = new mysqli('localhost', 'leaderuser', 'Lopat@_123455432', 'leaderboard');

if ($mysqli->connect_error) {
    echo "ERROR";
    exit;
}

$pseudo = $_GET['pseudo'] ?? '';

if (empty($pseudo) || strlen($pseudo) < 2) {
    echo "INVALID";
    exit;
}

$pseudo = $mysqli->real_escape_string($pseudo);

$result = $mysqli->query("SELECT score FROM scores WHERE player_name = '$pseudo' LIMIT 1");

if ($result->num_rows == 0)
{
    echo "PSEUDONOTFOUND";
    exit;
}

$myScore = $result->fetch_assoc()['score'];

echo $myScore;

$mysqli->close();