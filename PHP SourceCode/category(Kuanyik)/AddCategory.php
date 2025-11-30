<head>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="../js/app.js" type="text/javascript"></script>
    <script src="../js/script.js"></script>
    <link href="../css/kycss.css" rel="stylesheet" type="text/css"/>
    <link href="../css/app.css" rel="stylesheet" type="text/css"/>
    <title>Add New Category</title>
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

if (is_post()) {
    $categoryName = req('categoryName');
    $categoryInfo = req('categoryInfo');
    $f = get_file('photo');
    $f2 = get_file('photo2');

    // Validate: name
    if ($categoryName == '') {
        $_err['categoryName'] = '<span class="err-message">*Required</span>';
    } else if (strlen($categoryName) > 100) {
        $_err['categoryName'] = '<span class="err-message">*Maximum 100 characters</span>';
    } else if (!preg_match('/^[a-zA-Z\s]+$/', $categoryName)) {
        $_err['categoryName'] = '<span class="err-message">*Only alphabetic characters and spaces are allowed</span>';
    }
    // Validate: Info
    if ($categoryInfo == '') {
        $_err['categoryInfo'] = '<span class="err-message">*Required</span>';
    } else if (strlen($categoryInfo) > 555) {
        $_err['categoryInfo'] = '<span class="err-message">*Maximum 555 characters</span>';
    }

// Validate: photos (file)
    if (!$f || !$f2) {
        $_err['photos'] = '<span class="err-message">*Both images are required</span>';
    } else if (!str_starts_with($f->type, 'image/') || !str_starts_with($f2->type, 'image/')) {
        $_err['photos'] = '<span class="err-message">*Both images must be images</span>';
    } else if ($f->size > 1 * 1024 * 1024 || $f2->size > 1 * 1024 * 1024) {
        $_err['photos'] = '<span class="err-message">*Both images must be less than 1MB</span>';
    }


// DB operation
    if (!$_err) {
        // Save photo
        $photo = save_photo($f, '../photos');
        $photo2 = save_photo($f2, '../photos');

        $stm = $_db->prepare('
        INSERT INTO category (categoryName,categoryInfo,photo,photo2)
        VALUES (?, ?, ?, ?)
    ');
        $stm->execute([$categoryName, $categoryInfo, $photo, $photo2]);

        temp('success', '<span class="success-message">Record inserted successfully!</span>'); // Set a temporary success message
    }
}

// ----------------------------------------------------------------------------

$_title = 'Product | Insert';
?>

<div class="row clearfix">
    <div class="col-1" style="padding:0 200px;margin-bottom: 20px;text-align: center;">
        <h1>New Category</h1>
        <?= temp('success') ?>
    </div>
    <div class="col-1" style="padding: 0 200px;">
        <button data-get="EditCategory.php" class="reverse-button">Back</button>
    </div>
</div>

<div class="row clearfix nopadding-top">

    <div class="col-1">

        <form method="post" class="product-form" enctype="multipart/form-data" novalidate>

            <div class="col-1" style="margin: 0 0 20px 0;">
                <label class="category-label" for="categoryName">Category Name</label>                <?= err('categoryName') ?>
                <?= html_text('product-input', 'categoryName', 'Enter Category Name', 'maxlength="100"') ?>

            </div>

            <div class="col-1" style="margin: 0 0 20px 0;">
                <label class="category-label" for="categoryInfo">Category Information</label><?= err('categoryInfo') ?>
                <?= html_text('product-input', 'categoryInfo', 'Enter Category Information', 'maxlength="100"') ?>


            </div>
            <div class="col-1" style="margin-bottom:40px;">
                <label class="category-label">Select 2 Images</label><?= err('photos') ?>
                <br>
                <div style="display: flex;">
                    <label class="product-upload" tabindex="0" style="margin-right: 20px;">
                        <?= html_file('photo', 'image/*', 'hidden') ?>
                        <img src="../images/add.jpg">
                    </label>


                    <label class="product-upload" tabindex="0">
                        <?= html_file('photo2', 'image/*', 'hidden') ?>
                        <img src="../images/add.jpg">
                    </label>

                </div>
            </div>
            <section style="text-align: center; margin: 40px 0;">
                <button  type="reset" class="reverse-button">Reset</button>
                <button class="action-button">Submit</button>
            </section>
        </form>
    </div>
</div>
<?php
include '../admin/footer.php';
