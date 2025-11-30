
<!DOCTYPE html>
<html lang="en">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Category List</title>
        <link rel="shortcut icon" href="/images/favicon.png">
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
        <style>
            .popup {
                width: 100px;
                height: 100px;
            }
            .button-edit{
                display: block;
                border: 2px solid #ddd;
                border-radius:3px;
                background-color: #fff;
                padding:5px 10px;
                width: 100%;
                font-size: 14px;
                transition: all .2s;
            }
            .button-edit:hover{
                border: 2px solid #cfb595;
                border-radius:3px;
                color: white;
                background-color: #cfb595;
                padding:5px 10px;

            }
        </style>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
        <script src="../js/app.js" type="text/javascript"></script>
        <script src="../js/script.js"></script>
        <link href="../css/kycss.css" rel="stylesheet" type="text/css"/>
        <link href="../css/app.css" rel="stylesheet" type="text/css"/>
    </head>
    <?php
    include '../ky_base.php';
    $adminID = "";
    if (!isset($_SESSION)) {
        session_start();
    }
    if (isset($_SESSION['user']) && !empty($_SESSION['user'])) {
        $_user = $_SESSION['user'];
        $adminID = $_user->userID;
    } else {
        header("Location:../Home/HomePage.php");
    }
    include '../_head.php';
// ----------------------------------------------------------------------------

    $arr = $_db->query('SELECT * FROM category')->fetchAll();

// ----------------------------------------------------------------------------
//include '../_head.php';
    ?>



    <body>
        <div class="row clearfix">
            <div class="col-1" style="margin:10px 0;">
                <h1>Category List</h1>
            </div>
            <div class="col-1" style="">  
                <div class="main-category">
                    <a class="category-a" href="AddCategory.php" style="text-decoration: none;">
                        <div class="main-category-col fade-section" data-fade-duration="0.2s">
                            <img src="../images/add.jpg" alt="Bracelet-1" class="image"/>                            
                        </div>
                    </a>
                    <?php
                    $count = 0.4;
                    ?>
                    <?php foreach ($arr as $p): ?>
                        <div>
                            <a class="category-a" href="UpdateCategory.php?categoryID=<?= $p->categoryID ?>" style="text-decoration: none;">
                                <div class="main-category-col fade-section" data-fade-duration="<?= $count ?>s">
                                    <img src="../photos/<?= $p->photo ?>" alt="Bracelet-1" class="image"/>
                                    <div class="overlay">
                                        <img src="../photos/<?= $p->photo2 ?>" alt="Avatar" class="image" style="object-fit:cover;width:100%;height:100%;">
                                    </div>
                                </div>

                            </a>
                            <div style=" text-align:left; text-decoration: none;color:#333;">
                                <h2><?= $p->categoryName ?></h2>
                                <div style="display:flex;margin: 10px 0;">
                                    <button class="button-edit" data-get="UpdateCategory.php?categoryID=<?= $p->categoryID ?>">Update</button>
                                    <button class="button-edit" data-delete="DeleteCategory.php?categoryID=<?= $p->categoryID ?>">Delete</button></div>
                            </div>
                        </div>
                        <?php
                        $count += 0.2;
                        ?>
                    <?php endforeach ?>
                </div>
            </div>
        </div>

    </body>
    <?php include "../admin/footer.php" ?>
</html>