searchVisible = 0;
transparent = true;

$(document).ready(function(){
    /*  Activate the tooltips      */
    $('[rel="tooltip"]').tooltip();
      
    $('.wizard-card').bootstrapWizard({
        'tabClass': 'nav nav-pills',
        'nextSelector': '.btn-next',
        'previousSelector': '.btn-previous',
         
         onInit : function(tab, navigation, index){
            
           //check number of tabs and fill the entire row
           var $total = navigation.find('li').length;
           $width = 100/$total;
           
           $display_width = $(document).width();
           
           if($display_width < 600 && $total > 3){
               $width = 50;
           }
           
           navigation.find('li').css('width',$width + '%');
           
        },
        onNext: function(tab, navigation, index){
            if(index == 1){
                return validateFirstStep();
            } else if(index == 2){
                return validateSecondStep();
            } else if(index == 3){
                return validateThirdStep();
            } //etc. 
              
        },
        onTabClick : function(tab, navigation, index){
            // Disable the posibility to click on tabs
            return false;
        }, 
        onTabShow: function(tab, navigation, index) {
            var $total = navigation.find('li').length;
            var $current = index+1;
            
            var wizard = navigation.closest('.wizard-card');
            
            // If it's the last tab then hide the last button and show the finish instead
            if($current >= $total) {
                $(wizard).find('.btn-next').hide();
                $(wizard).find('.btn-finish').show();
            } else {
                $(wizard).find('.btn-next').show();
                $(wizard).find('.btn-finish').hide();
            }
        }
    });

    // Prepare the preview for profile picture
    $("#wizard-picture").change(function(){
        readURL(this);
    });
    
    $('[data-toggle="wizard-radio"]').click(function(){
        wizard = $(this).closest('.wizard-card');
        wizard.find('[data-toggle="wizard-radio"]').removeClass('active');
        $(this).addClass('active');
        $(wizard).find('[type="radio"]').removeAttr('checked');
        $(this).find('[type="radio"]').attr('checked','true');
    });
    
    $('[data-toggle="wizard-checkbox"]').click(function(){
        if( $(this).hasClass('active')){
            $(this).removeClass('active');
            $(this).find('[type="checkbox"]').removeAttr('checked');
        } else {
            $(this).addClass('active');
            $(this).find('[type="checkbox"]').attr('checked','true');
        }
    });
    
    $height = $(document).height();
    $('.set-full-height').css('height',$height);
    
    
});

function validateFirstStep(){
    
    $(".wizard-card form").validate({
		rules: {
			firstname: "required",
			lastname: "required",
			email: {
				required: true,
				email: true
			}
			
/*  other possible input validations
			,username: {
				required: true,
				minlength: 2
			},
			password: {
				required: true,
				minlength: 5
			},
			confirm_password: {
				required: true,
				minlength: 5,
				equalTo: "#password"
			},
		
			topic: {
				required: "#newsletter:checked",
				minlength: 2
			},
			agree: "required"
*/			

		},
		messages: {
			firstname: "Please enter your First Name",
			lastname: "Please enter your Last Name",
			email: "Please enter a valid email address",

/*   other posible validation messages
			username: {
				required: "Please enter a username",
				minlength: "Your username must consist of at least 2 characters"
			},
			password: {
				required: "Please provide a password",
				minlength: "Your password must be at least 5 characters long"
			},
			confirm_password: {
				required: "Please provide a password",
				minlength: "Your password must be at least 5 characters long",
				equalTo: "Please enter the same password as above"
			},
			email: "Please enter a valid email address",
			agree: "Please accept our policy",
			topic: "Please select at least 2 topics"
*/
				
		}
	}); 
	
	if(!$(".wizard-card form").valid()){
    	//form is invalid
    	return false;
	}
	
	return true;
}

function validateSecondStep(){
   
    //code here for second step
    $(".wizard-card form").validate({
		rules: {
			
		},
		messages: {
			
		}
	}); 
	
	if(!$(".wizard-card form").valid()){
    	console.log('invalid');
    	return false;
	}
	return true;
    
}

function validateThirdStep(){
    //code here for third step
    
}

 //Function to show image before upload

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#wizardPicturePreview').attr('src', e.target.result).fadeIn('slow');
        }
        reader.readAsDataURL(input.files[0]);
    }
}
    











function tblMeasureValues() {
    var TableData = new Array();

    $('#tblMeasures>tbody tr').each(function (row, tr) {
        TableData[row] = {
            "measure": $(tr).find("#measure").val()
            , "width": $(tr).find("#width").val()
            , "length": $(tr).find("#length").val()
        }
    });
    //TableData.shift();  // first row will be empty - so remove
    TableData.filter(Boolean);
    TableData.filter(function (v) { return v !== '' });
    return TableData;
}

function tblSinkValues() {
    var TableData = new Array();

    $('#tblSinks>tbody tr').each(function (row, tr) {
        TableData[row] = {
            "sinkid": $(tr).find('#sinkid').val()
            , "quantity": $(tr).find('#sinkqty').val()
            , "price": $(tr).find('#sinkprice').val()
        }
    });
    //TableData.shift();  // first row will be empty - so remove
    TableData.filter(Boolean);
    TableData.filter(function (v) { return v !== '' });
    return TableData;
}

function tblHoldStoneValues() {
    var TableData = new Array();

    $('#InventoryTable>tbody tr.trhold').each(function (row, tr) {
        TableData[row] = {
            "Id": $(tr).find('#HoldStones').val(),
            "EdgeId": $(tr).find('#HoldEdges').val()
        }
    });
    //TableData.shift();  // first row will be empty - so remove
    TableData.filter(Boolean);
    TableData.filter(function (v) { return v !== '' });
    return TableData;
}


function tblHoldSinkValues() {
    var TableData = new Array();

    $('#SinkInventoryTable>tbody tr.trhold').each(function (row, tr) {
        TableData[row] = {
            "Id": $(tr).find('#HoldSinks').val(),
        }
    });
    //TableData.shift();  // first row will be empty - so remove
    TableData.filter(Boolean);
    TableData.filter(function (v) { return v !== '' });
    return TableData;
}


$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};


$(".btn-finish").click(function () {
    //$('#cancel-quote').prop('disabled', true);   

    //var l = Ladda.create(this);
    //l.start();

    $(".btn-finish").val('Submitting...');
    $(".btn-finish").prop('disabled', true);

    var tableMeasures, tableSinks, tableStones, customerInfo;
    tableMeasures = JSON.stringify(tblMeasureValues()); //$.toJSON
    tableStones = JSON.stringify(tblHoldStoneValues());
    tableSinks = JSON.stringify(tblHoldSinkValues());
    customerInfo = JSON.stringify($('form').serializeObject())
    //alert('JSON array to send to server: \n\n' + TableData.replace(/},/g, "},\n"));
    //alert(TableData);

    $.ajax({
        url: ROOT + "Quote/SubmitWizard",
        type: 'POST',
        dataType: 'text',
        data: "jsonCustomer=" + customerInfo
            + "&jsonMeasures=" + tableMeasures
            + "&jsonStones=" + tableStones
            + "&jsonSinks=" + tableSinks
            //+ "&SlabColorID=" + encodeURIComponent($('#ddlSlabColorID').val())
            //+ "&SFQty=" + encodeURIComponent($('#SFQuantity').val())
            //+ "&EdgeID=" + encodeURIComponent($('#ddlEdges').val())
            //+ "&jsonSinks=" + tableSinks
            //+ "&jsonServices=" + tableServices
            //+ "&jsonCustomer=" + customerInfo
            //+ "&Notes=" + encodeURIComponent($('#notes').val())
            //+ "&SlabPromoOveride=" + encodeURIComponent($('#SlabPromoOveride').val())

    })
    .done(function (data) {
        if (data == 'err') {
            alert('Oops! We encountered a problem creating quote. Please try again.');
            $(".btn-finish").prop('disabled', false);
            $(".btn-finish").val('Try again');
        }
        else {
            window.location = data;
        }
    })
    .fail(function (xhr, ajaxOptions, thrownError) {
        console && console.log("request failed");
    });
});


