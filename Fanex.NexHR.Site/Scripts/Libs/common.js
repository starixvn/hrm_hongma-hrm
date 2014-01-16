/// <reference path="jquery.min.js" />
/// <reference path="date.js" />

(function() {
	var proxied = window.alert;
	window.alert = function() {
		$('<div class="dialogcontent">' + arguments[0] + '<input class="dialogokbutton" type="button" value="OK" onclick="$(\'.ui-dialog-content\').dialog(\'close\');" /></div>').dialog({
			title: 'NexHRM',
			height: 120,
			show: 'fade',
			hide: 'fade',
			modal: true
		});
	};
})();

$.extend({
	getOrCreateDialog: function(id) {
		$box = $('#' + id);
		if (!$box.length) {
			$box = $('<div id="' + id + '"><p></p></div>').hide().appendTo('body');
		}
		return $box;
	}
});

function confirm(message, callback, options) {
	var defaults = {
		title: 'NexHRM',
		modal		: true,
		resizable	: false,
		buttons		: {
			OK: function() {
				$(this).dialog('close');
				if(callback != null)
				{
					callback();
				}
				return true;
			},
			Cancel: function() {
				$(this).dialog('close');
				return false;
			}
		},
		show		: 'fade',
		hide		: 'fade',
		minHeight	: 50,
		dialogClass	: 'modal-shadow'
	}
	
	$confirm = $.getOrCreateDialog('confirm');
	// set message
	$("p", $confirm).html(message);
	// init dialog
	$confirm.dialog($.extend({}, defaults, options));
}

$(document).ready(function(){

	$('.tooltip').tipsy({
		live: true,
		gravity: $.fn.tipsy.autoNS
	});

	$('.flash').animate({opacity: 0.25},3500, function(){
		$(this).slideUp().remove();
	});

	$('#searchbox').keyup(function (event) {
		if (event.keyCode === 13) {
			document.location = _global.root + 'employee/index?query=' + $(this).val();
		}
	});

	//Handle event when click search icon
	$("#searchIcon").click(function(){
		document.location = _global.root + 'employee/index?query=' + $("#searchbox").val();
	});

	$("input[type=number]").keydown(function(event) {
		// Allow: backspace, delete, tab, escape, and enter
		if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode === 13 ||
			// Allow: Ctrl+A
			(event.keyCode === 65 && event.ctrlKey === true) ||
			// Allow: home, end, left, right
			(event.keyCode >= 35 && event.keyCode <= 39)) {
			// let it happen, don't do anything
			return;
		}
		else {
			// Ensure that it is a number and stop the keypress
			if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105 )) {
				event.preventDefault();
			}
		}
	});

	if($('.datepicker').length > 0) {
		// get minDate, maxDate, disabledDates
		var mindate = $('.datepicker').attr('mindate');
		var maxdate = $('.datepicker').attr('maxdate');
		var disabledDates = $('.datepicker').attr('disabledDate');

		if(disabledDates.length > 0) {
			$('.datepicker').datepicker({
				dateFormat: "dd M, yy",
				beforeShowDay: checkBadDates,
				showOn: "button",
				buttonImageOnly: true,
				buttonImage: _global.root + "images/calendar.png"
			});
		} 
		else {
			$('.datepicker').datepicker({
				dateFormat: "dd M, yy",
				showOn: "button",
				buttonImageOnly: true,
				buttonImage: _global.root + "images/calendar.png"
			});
		}

		if(typeof(initDatePicker) != "function") //check if (isExit include date.filter.js) then (mindate, maxdate include after function initDatePicker)
		{
			if (mindate != 'null') {
				mindate = mindate.split("/");
				$('.datepicker').datepicker("option", "minDate", new Date(mindate[0], mindate[1] - 1, mindate[2]));
			}
			if (maxdate != 'null') {
				maxdate = maxdate.split("/");
				$('.datepicker').datepicker("option", "maxDate", new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
			}
		}
	}

	var clonedHeaderRow;
	$(".persist-area").each(function () {
		clonedHeaderRow = $(".persist-header", this);
		clonedHeaderRow
			.before(clonedHeaderRow.clone())
			.css("width", clonedHeaderRow.width())
			.addClass("floatingHeader")
			.removeClass("persist-header");
	});

	$(".content_scroll")
		.scroll(updateTableHeaders)
		.trigger("scroll");
	$(window).resize(function() {
		updateTableHeaders();
	});

	$('.processing-submit').click(function (evt) {
		actingButton('.processing-submit', true);
	});
});


function checkBadDates(mydate) {
	var isEnable = true;
	var disabledDates = $('.datepicker').attr('disabledDate');
	var myBadDates = disabledDates.split(",");

	var checkdate = new Date(mydate).format("dd mmm, yyyy");
	for (var i = 0; i < myBadDates.length; i++) {
		var date = new Date(myBadDates[i]).format("dd mmm, yyyy");
		if (date == checkdate) {
			isEnable = false;
		}
	}

	return [isEnable];
}

//required: jquery.tipsy.js
function tooltip(element, enable) {
	if (enable) {
		$(element).tipsy({
		live: false,
		gravity: $.fn.tipsy.autoNS
	});
	}
	else {
		$(element).tipsy("hide");
	}
}

function getParam(name) {
	name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
	var regexS = "[\\?&]" + name + "=([^&#]*)";
	var regex = new RegExp(regexS);
	var results = regex.exec(window.location.href);
	if (results == null)
		return null;
	else
		return results[1];
}

function stringToBool(value)
{
	switch(value)
	{
		case 'null':
			return null;
		case 'false':
			return false;
		case 'true':
			return true;
	}
}

function showMessage(message) {
	$('#message').html(message + ' <img src="/images/ajax-loader.gif" />').css('left',$(window).width()/2);
	$('#message').slideDown();
}

function fadeOutMessage() {
	$('#message').fadeOut(500);
}

function hideMessage() {
	$('#message').text('Done');
	$('#message').fadeOut(500);
}

function flashMessage(message) {
	$('#message').hide();
	$('#message').html(message);
	$('#message').slideDown(function () {
		$('#message').fadeOut(3500);
	});
}

function actingButton(element, disabled)
{
	$(element).attr("origin-value", $(element).attr("value"));
	$(element).attr("value", 'Processing');
//    $(element).attr("disabled", "disabled");
	$(element).addClass("disabled");
}
function reloadButtonText(element)
{
	$(element).attr("value", $(element).attr("origin-value"));
//    $(element).removeAttr("disabled");
	$(element).removeClass("disabled");
}

function getDomain(url) {
	return url.match(/:\/\/(www\.)?(.[^/:]+)/)[2];
}
function getPlainText(text) {
	if(text == null || typeof(text) != "string") return null;
	return text.replace(/<[^>]*>/g, '');
}

function getEncodePlainText(text) {
	if(text == null || typeof(text) != "string") return null;
	return encodeURIComponent(text.replace(/<[^>]*>/g, ''));
}
function jsonToDate(jsonDate) {
	var date = Date.parse(jsonDate);
	return date.format("mm/dd/yyyy HH:MM:ss");
	//return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear() + " "
    //+ date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds();
}

function jsonToShortDate(jsonDate) {
	var date = Date.parse(jsonDate);
	return date.format("dd mmm, yyyy");
}

function jsonTojsDate(jsonDate) {
	return Date.parse(jsonDate);
//	return new Date(parseInt(jsonDate.substr(6)));
}

function dateToString(date) {
	return date.format("mm/dd/yyyy HH:MM:ss");
}

function initEditor(element, type) {
	switch (type) {
		case "basic":
			tinyMCE.init({
				mode: "exact",
				elements: element,
				theme: "advanced",
				theme_advanced_toolbar_location: "top",
				theme_advanced_toolbar_align: "left",
				theme_advanced_statusbar_location: "bottom",
				plugins: "safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

				theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,fontselect,fontsizeselect,|,forecolor,backcolor",
				theme_advanced_buttons2: "bullist,numlist,|,outdent,indent,|,link,unlink,image,|,table,|,hr,|,sub,sup,|,charmap,emotions,|,ltr,rtl",
				theme_advanced_buttons3: "",
				content_css: '/styles/tinymce.css',
				force_br_newlines: true,
				force_p_newlines: false
			});
			break;
		case "full":
			tinyMCE.init({
				mode: "exact",
				elements: element,
				theme: "advanced",
				theme_advanced_toolbar_location: "top",
				theme_advanced_toolbar_align: "left",
				theme_advanced_statusbar_location: "bottom",
				plugins: "safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template",

				theme_advanced_buttons1: "bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect,|,forecolor,backcolor",
				theme_advanced_buttons2: "pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,code,|,insertdate,inserttime,preview,spellchecker",
				theme_advanced_buttons3: "table,|,hr,removeformat,|,sub,sup,|,charmap,emotions,iespell,media,|,ltr,rtl,|,fullscreen",
				content_css: '/styles/tinymce.css',
				force_br_newlines: false,
				force_p_newlines: true
			});
			break;
	}
}

function removeSign(str) {
	str = str.toLowerCase();
	str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
	str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
	str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
	str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
	str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
	str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
	str = str.replace(/đ/g, "d");
	str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
	str = str.replace(/-+-/g, "-"); //change -- become -
	str = str.replace(/^\-+|\-+$/g, ""); //remove - at begin and end of string
	return str;
}

function enableEmotion(str) {
	var path = '<img src="/images/emoticon/';
	var endClose = '.gif" />';
	str = str.replace(/\n/g, '<br />');
	str = str.replace(/\*:\)/g, path + 'devilish' + endClose);
	str = str.replace(/:\)\)/g, path + 'laughing' + endClose);
	str = str.replace(/:\)/g, path + 'smile' + endClose);
	str = str.replace(/o:-\)/g, path + 'angel' + endClose);
	str = str.replace(/x-\(/g, path + 'angry' + endClose);
	str = str.replace(/:D|:d/g, path + 'biggrin' + endClose);
	str = str.replace(/=\(\(/g, path + 'brokenheart' + endClose);
	str = str.replace(/:\(\(/g, path + 'crying' + endClose);

	str = str.replace(/=p~/g, path + 'drooling' + endClose);
	str = str.replace(/:\(/g, path + 'frown' + endClose);
	str = str.replace(/:-\*/g, path + 'kiss' + endClose);

	str = str.replace(/:x/g, path + 'lovestruck' + endClose);
	str = str.replace(/\*:p/g, path + 'phbbbt' + endClose);
	str = str.replace(/\[-O\*/g, path + 'praying' + endClose);
	str = str.replace(/:-&/g, path + 'puke' + endClose);
	str = str.replace(/=\)\)/g, path + 'rolling' + endClose);
	str = str.replace(/:-p/g, path + 'tongue' + endClose);
	str = str.replace(/#:-s/g, path + 'whew' + endClose);
	str = str.replace(/:-s/g, path + 'worried' + endClose);
	return str;
}

function getImage(text) {
	text = text.replace(/\)|\(|&quot;/g, "");
	var img = $(text).find("img:first").attr("src");
	if(img != null && img != undefined)
	{
		return "<img alt='' src='" + img + "' />";
	}
	else
	{
		return '';
	}
}

/*
 * Date Format 1.2.3
 * (c) 2007-2009 Steven Levithan <stevenlevithan.com>
 * MIT license
 *
 * Includes enhancements by Scott Trenda <scott.trenda.net>
 * and Kris Kowal <cixar.com/~kris.kowal/>
 *
 * Accepts a date, a mask, or a date and a mask.
 * Returns a formatted version of the given date.
 * The date defaults to the current date/time.
 * The mask defaults to dateFormat.masks.default.
 */

var dateFormat = function () {
	var	token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
		timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
		timezoneClip = /[^-+\dA-Z]/g,
		pad = function (val, len) {
			val = String(val);
			len = len || 2;
			while (val.length < len) val = "0" + val;
			return val;
		};

	// Regexes and supporting functions are cached through closure
	return function (date, mask, utc) {
		var dF = dateFormat;

		// You can't provide utc if you skip other args (use the "UTC:" mask prefix)
		if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
			mask = date;
			date = undefined;
		}

		// Passing date through Date applies Date.parse, if necessary
		date = date ? new Date(date) : new Date;
		if (isNaN(date)) throw SyntaxError("invalid date");

		mask = String(dF.masks[mask] || mask || dF.masks["default"]);

		// Allow setting the utc argument via the mask
		if (mask.slice(0, 4) == "UTC:") {
			mask = mask.slice(4);
			utc = true;
		}

		var	_ = utc ? "getUTC" : "get",
			d = date[_ + "Date"](),
			D = date[_ + "Day"](),
			m = date[_ + "Month"](),
			y = date[_ + "FullYear"](),
			H = date[_ + "Hours"](),
			M = date[_ + "Minutes"](),
			s = date[_ + "Seconds"](),
			L = date[_ + "Milliseconds"](),
			o = utc ? 0 : date.getTimezoneOffset(),
			flags = {
				d:    d,
				dd:   pad(d),
				ddd:  dF.i18n.dayNames[D],
				dddd: dF.i18n.dayNames[D + 7],
				m:    m + 1,
				mm:   pad(m + 1),
				mmm:  dF.i18n.monthNames[m],
				mmmm: dF.i18n.monthNames[m + 12],
				yy:   String(y).slice(2),
				yyyy: y,
				h:    H % 12 || 12,
				hh:   pad(H % 12 || 12),
				H:    H,
				HH:   pad(H),
				M:    M,
				MM:   pad(M),
				s:    s,
				ss:   pad(s),
				l:    pad(L, 3),
				L:    pad(L > 99 ? Math.round(L / 10) : L),
				t:    H < 12 ? "a"  : "p",
				tt:   H < 12 ? "am" : "pm",
				T:    H < 12 ? "A"  : "P",
				TT:   H < 12 ? "AM" : "PM",
				Z:    utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
				o:    (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
				S:    ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
			};

		return mask.replace(token, function ($0) {
			return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
		});
	};
}();

// Some common format strings
dateFormat.masks = {
	"default":      "ddd mmm dd yyyy HH:MM:ss",
	shortDate:      "m/d/yy",
	mediumDate:     "mmm d, yyyy",
	longDate:       "mmmm d, yyyy",
	fullDate:       "dddd, mmmm d, yyyy",
	shortTime:      "h:MM TT",
	mediumTime:     "h:MM:ss TT",
	longTime:       "h:MM:ss TT Z",
	isoDate:        "yyyy-mm-dd",
	isoTime:        "HH:MM:ss",
	isoDateTime:    "yyyy-mm-dd'T'HH:MM:ss",
	isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
};

// Internationalization strings
dateFormat.i18n = {
	dayNames: [
		"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
		"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
	],
	monthNames: [
		"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
		"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
	]
};

// For convenience...
Date.prototype.format = function (mask, utc) {
	return dateFormat(this, mask, utc);
};

jQuery.cookie = function (key, value, options) {
	// key and at least value given, set cookie...
	if (arguments.length > 1 && String(value) !== "[object Object]") {
		options = jQuery.extend({}, options);

		if (value === null || value === undefined) {
			options.expires = -1;
		}

		if (typeof options.expires === 'number') {
			var days = options.expires, t = options.expires = new Date();
			t.setDate(t.getDate() + days);
		}

		value = String(value);

		return (document.cookie = [
			encodeURIComponent(key), '=',
			options.raw ? value : encodeURIComponent(value),
			options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
			options.path ? '; path=' + options.path : '',
			options.domain ? '; domain=' + options.domain : '',
			options.secure ? '; secure' : ''
		].join(''));
	}

	// key and possibly options given, get cookie...
	options = value || {};
	var result, decode = options.raw ? function (s) { return s; } : decodeURIComponent;
	return (result = new RegExp('(?:^|; )' + encodeURIComponent(key) + '=([^;]*)').exec(document.cookie)) ? decode(result[1]) : null;
};

/*insert to text area*/
$.fn.insertAtCaret = function (myValue) {
	return this.each(function () {
		//IE support
		if (document.selection) {
			this.focus();
			sel = document.selection.createRange();
			sel.text = myValue;
			this.focus();
		}
		//MOZILLA/NETSCAPE support
		else if (this.selectionStart || this.selectionStart == '0') {
			var startPos = this.selectionStart;
			var endPos = this.selectionEnd;
			var scrollTop = this.scrollTop;
			this.value = this.value.substring(0, startPos)
							+ myValue
							+ this.value.substring(endPos, this.value.length);
			this.focus();
			this.selectionStart = startPos + myValue.length;
			this.selectionEnd = startPos + myValue.length;
			this.scrollTop = scrollTop;
		} else {
			this.value += myValue;
			this.focus();
		}
	});
};

function changeNumber(element, amount) {
	var text = $(element).text();
	var r = text.match(/[\d\.]+/g);
	if(text.length == 0 || r == null) return;

	var next = parseInt(r[0]) + amount;
	$(element).text(text.replace(r[0],next));
}

function  isInteger(str) {
	return /^ *[0-9]+ *$/.test(str);
}

function displayMinute(minutes) {
	var hour = Math.floor(minutes / 60);
	var minute = minutes % 60;
	if(hour == 0)
	{
		return minute > 9 ? '00:' + minute : '00:0' + minute;
	}
	return (hour > 9 ? hour : '0' + hour) + ':' + (minute > 9 ? minute : '0' + minute);
}

function noInformation(col) {
	return '<tr><td colspan="'+col+'"></td></tr><tr><td colspan="'+col+'">No information available</td></tr>';
}

function autoIndex() {
	var index = 0;
	$('tr').each(function () {
		$(this).children("td").eq(0).html(index);
		index = index + 1;
	});
}

function zeroPrefix(value) {
	if(parseInt(value) < 10) return "0" + parseInt(value);
	return value;
}

function totalHoursToTime(value) {
	var display = '';
	var hour = Math.floor(value);

	if(hour > 0)
	{
		display = hour < 10 ? '0' + hour + 'h' : hour + 'h';
	}

	var minute = Math.round((value - hour) * 60);
	return minute < 10 ? display + '0' + minute + '\'' : display + minute + '\'';
}

function updateTableHeaders() {
	if($(".persist-area").val() != undefined)
	{
		var leftspace= 191; 
		var widthspace = $(".persist-area").width() + 18;
		if($.browser.safari)
		{
			leftspace=192;
			widthspace = widthspace - 8;
			if(widthspace < 1060)
				widthspace = 1060;
		}
		else
		{
			widthspace = widthspace - 17;
			if(widthspace < 1051)
				widthspace = 1051;
		}

		$('.floatingHeader').width(widthspace);
		var left = $('table thead tr:first').position()["left"];
		$(".persist-area").each(function () {
			var el = $(this),
					offset = el.offset(),
					scrollTop = $(window).scrollTop(),
					floatingHeader = $(".floatingHeader", this)
			if ((scrollTop > offset.top -60) && (scrollTop < offset.top + el.height())) {
				floatingHeader.css({
					"visibility": "visible",
					"left": (leftspace+left)
				});
			} else {
				floatingHeader.css({
					"visibility": "hidden"
				});
			};
		});

		var i = 0;
		$("table .persist-header th").each(function () {
			var x = $(this).width();
			$($("table .floatingHeader th")[i]).width(x);
			i++;
		});
//        if($.browser.mozilla)
//            $("table .floatingHeader th:first").height(30);
    }
}
