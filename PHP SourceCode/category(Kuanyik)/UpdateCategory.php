<head>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="../js/app.js" type="text/javascript"></script>
    <script src="../js/script.js"></script>
    <link href="../css/kycss.css" rel="stylesheet" type="text/css"/>
    <link href="../css/app.css" rel="stylesheet" type="text/css"/>
</head>
<?php
include '../ky_base.php';

// ----------------------------------------------------------------------------

if (is_get()) {
    $categoryID = req('categoryID');

    $stm = $_db->prepare('SELECT * FROM category WHERE categoryID = ?');
    $stm->execute([$categoryID]);
    $p = $stm->fetch();

    if (!$p) {
        redirect('EditCategory.php');
    }

    extract((array) $p);
    $_SESSION['photo'] = $p->photo;
    $_SESSION['photo2'] = $p->photo2;
}

if (is_post()) {
    $categoryID = req('categoryID');
    $categoryName = req('categoryName');
    $categoryInfo = req('categoryInfo');
    $f = get_file('photo');
    $f2 = get_file('photo2');
    $photo = $_SESSION['photo']; // TODO
    $photo2 = $_SESSION['photo2']; // TODO
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

    // Validate: photo (file)
    // ** Only if a file is selected **
// Validate: photos (file)
    if ($f || $f2) {
        if (isset($f) && !str_starts_with($f->type, 'image/')) {
            $_err['photos'] = '<span class="err-message">*First image must be an image</span>';
        }
        if (isset($f2) && !str_starts_with($f2->type, 'image/')) {
            $_err['photos'] = '<span class="err-message">*Second image must be an image</span>';
        }
        if (isset($f) && $f->size > 1 * 1024 * 1024) {
            $_err['photos'] = '<span class="err-message">*First image must be less than 1MB</span>';
        }
        if (isset($f2) && $f2->size > 1 * 1024 * 1024) {
            $_err['photos'] = '<span class="err-message">*Second image must be less than 1MB</span>';
        }
    }

    // DB operation
    if (!$_err) {
        // Delete old photo + save new photo
        if ($f) {
            unlink("../photos/$photo");
            $photo = save_photo($f, '../photos');
        }
        if ($f2) {
            unlink("../photos/$photo2");
            $photo2 = save_photo($f2, '../photos');
        }

        $stm = $_db->prepare('
        UPDATE category
        SET categoryName = ?, categoryInfo = ?, photo = ?, photo2 = ?
        WHERE categoryID = ?
    ');
        $stm->execute([$categoryName, $categoryInfo, $photo, $photo2, $categoryID]);

        temp('success', '<span class="success-message">Record Updated successfully!</span>'); // Set a temporary success message
        redirect("UpdateCategory.php?categoryID=$categoryID");
    }
}

// ----------------------------------------------------------------------------

$_title = 'Product | Update';
include '../_head.php';
?>
<body>
    <div class="row clearfix">
        <div class="col-1" style="padding:0 200px;margin-bottom: 20px;text-align: center;">
            <h1>Update Category</h1>
            <?= temp('success') ?>
        </div>
<!--        <div class="col-1" style="padding: 0 200px;">
            <button data-get="EditCategory.php" class="reverse-button">Back</button>
        </div>-->
    </div>


    <div class="row clearfix nopadding-top">
        <div class="col-1">
            <form method="post" class="product-form" enctype="multipart/form-data" novalidate>

                <div class="col-1" style="margin: 0 0 20px 0;">
                    <label for="categoryName" class="category-label">Category Name</label>  <?= err('categoryName') ?>
                    <br>
                    <?= html_text('product-input', 'categoryName', 'Enter CategoryName', 'maxlength="100"') ?>

                </div>

                <div class="col-1" style="margin: 0 0 20px 0;">
                    <label for="categoryInfo"  class="category-label">Category Information</label>  <?= err('categoryInfo') ?>
                    <br>
                    <?= html_text('product-input', 'categoryInfo', 'Enter Category Information', 'maxlength="555"') ?>

                </div>
                <div class="col-1" style="margin-bottom: 40px;">
                    <label  class="category-label">Select 2 Images</label><?= err('photos') ?>
                    <br>
                    <div style="display: flex;">
                        <label class="product-upload" tabindex="0" style="margin-right: 20px;">
                            <?= html_file('photo', 'image/*', 'hidden') ?>
                            <img src="../photos/<?= $photo ?>">
                        </label>
                        <label class="product-upload" tabindex="0">
                            <?= html_file('photo2', 'image/*', 'hidden') ?>
                            <img src="../photos/<?= $photo2 ?>">
                        </label>
                        <?= err('photo2') ?>
                    </div>
                </div>
                <section style="text-align: center; margin: 40px 0;">
                    <button  type="reset" class="reverse-button">Reset</button>
                    <button class="action-button">Submit</button>
                </section>
            </form>
        </div>
    </div>
<?php include'../admin/footer.php'; ?>
</body>
</html>
