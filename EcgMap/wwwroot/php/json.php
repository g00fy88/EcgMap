<?php

$text = file_get_contents($_GET['url']);
$clean = preg_replace( '/[\x{200B}-\x{200D}\x{FEFF}]/u', '', $text );
echo $clean;
?>