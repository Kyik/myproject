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
//-----------------------------------------------------------------------------

$categoryName = $_db->query('SELECT categoryID, categoryName FROM category')
        ->fetchAll(PDO::FETCH_KEY_PAIR);
$name = req('productName');
$program_id = req('categoryID');

// Fetch products based on the search criteria
$stm = $_db->prepare('SELECT * FROM product
                      WHERE productName LIKE ?
                      AND (categoryID = ? OR ?)');
$stm->execute(["%$name%", $program_id, $program_id == null]);
$arr = $stm->fetchAll();

// Prepare to get sizes data for each product
$sizes = []; // Array to hold sizes data

foreach ($arr as $p) {
    // Fetch sizes for the current product
    $stmSizes = $_db->prepare('SELECT size, quantity FROM sizes WHERE productID = ?');
    $stmSizes->execute([$p->productID]);
    $sizes[$p->productID] = $stmSizes->fetchAll(PDO::FETCH_ASSOC);
}

// ----------------------------------------------------------------------------
$_title = 'Demo 3 | Combined';
?>
<head>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="../js/app.js" type="text/javascript"></script>
    <script src="../js/script.js"></script>
    <link href="../css/kycss.css" rel="stylesheet" type="text/css"/>
    <link href="../css/app.css" rel="stylesheet" type="text/css"/>
    <title>Product List</title>
</head>
<body>
    <div class="row clearfix">
        <div class="col-1">
            <form>
                <?= html_search('product-input', 'productName', 'Search Product Name', '') ?>
                <br><br>
                <?= html_select('product-input', 'categoryID', $_programs, 'All') ?><br><br>
                <button class="button-edit">Search</button>
            </form>
        </div>
        <div class="col-1" style="margin:10px 0;">
            <h1>Product List</h1>
        </div>
        <div class="col-1" style="padding: 0px;margin: 20px 0;">
            <button data-get="../admin/productMain2.php" class="reverse-button">Back</button>
        </div>
        <div class="col-1" style="">  
            <div class="main-category">
                <a class="category-a" href="AddProduct.php" style="text-decoration: none;">
                    <div class="main-category-col fade-section" data-fade-duration="0.2s">
                        <img src="../images/add.jpg" alt="Bracelet-1" class="image"/>                            
                    </div>
                </a>
                <?php
                $count = 0.4;
                foreach ($arr as $p):
                    ?>
                    <div>
                        <a class="category-a" href="UpdateProduct.php?productID=<?= $p->productID ?>" style="text-decoration: none;">
                            <div class="main-category-col fade-section" data-fade-duration="<?= $count ?>s">
                                <img src="../photos/<?= $p->productPhoto1 ?>" alt="Bracelet-1" class="image"/>
                                <div class="overlay">
                                    <img src="../photos/<?= $p->productPhoto2 ?>" alt="Avatar" class="image" style="object-fit:cover;width:100%;height:100%;">
                                </div>
                            </div>
                        </a>
                        <div style="text-align:left; text-decoration: none;color:#333;">
                            <h3><?= $categoryName[$p->categoryID] ?></h3>
                            <h4><?= $p->productName ?></h4>

                            <?php
                            // Check for low stock sizes
                            $lowStockSizes = [];
                            foreach ($sizes[$p->productID] as $size) {
                                if ($size['quantity'] <= 2) {
                                    $lowStockSizes[] = $size['size'];
                                }
                            }

                            if (!empty($lowStockSizes)):
                                ?>
                                <p style="color: red;">Stock is Low for size(s): <?= implode(', ', $lowStockSizes) ?></p>
    <?php endif; ?>

                            <div style="display:flex;margin: 10px 0;">
                                <button class="button-edit" data-get="UpdateProduct.php?productID=<?= $p->productID ?>">Update</button>
                                <button class="button-edit" data-delete="DeleteProduct.php?productID=<?= $p->productID ?>">Delete</button>
                            </div>
                        </div>
                    </div>
                    <?php
                    $count += 0.2;
                endforeach;
                ?>
            </div>
        </div>
    </div>

<?php
include '../admin/footer.php';
