

var intl = new ej.base.Internationalization();
window.getDate = function (value) {
	return intl.formatDate(value, { skeleton: 'yMd', type: 'date' });
};
window.getCurrencyVal = function (value) {
	return intl.formatNumber(value, { format: 'C0' });
};
window.getNumberVal = function (value) {
	return intl.getNumberFormat({ skeleton: 'C0', currency: 'USD' })(value);
};

var dataSource = [];
var menu = HTMLElement;
var overlay = HTMLElement;

var FileManagerDashboard;
(function (FileManagerDashboard) {
	FileManagerDashboard[FileManagerDashboard["index"] = 0] = "index";
	FileManagerDashboard[FileManagerDashboard["userEvent"] = 1] = "userEvent";
	FileManagerDashboard[FileManagerDashboard["userNonEvent"] = 2] = "userNonEvent";
	FileManagerDashboard[FileManagerDashboard["events"] = 3] = "events";
	FileManagerDashboard[FileManagerDashboard["dashboard"] = 4] = "dashboard";
	FileManagerDashboard[FileManagerDashboard["accounts"] = 5] = "accounts";
	FileManagerDashboard[FileManagerDashboard["cabangs"] = 6] = "cabangs";
	FileManagerDashboard[FileManagerDashboard["filemanagers"] = 7] = "filemanagers";

})(FileManagerDashboard || (FileManagerDashboard = {}));

console.log(FileManagerDashboard);

/* tslint:disable-next-line */
function updateDate(list) {
	dataSource = list;
}
/* tslint:disable-next-line */
function parseDate(date) {
	return new Date((date).match(/\d+/)[0] * 1);
}
handleResize();
function getCurrentPage() {
	// var currentPage;
	if ((window.location.hash === '#/' + FileManagerDashboard[FileManagerDashboard.dashboard])) {
		currentPage = FileManagerDashboard[FileManagerDashboard.dashboard];
	} else if ((window.location.hash === '#/' + FileManagerDashboard[FileManagerDashboard.userEvent])) {
		currentPage = FileManagerDashboard[FileManagerDashboard.userEvent];
	} else if ((window.location.hash === '#/' + FileManagerDashboard[FileManagerDashboard.userNonEvent])) {
		currentPage = FileManagerDashboard[FileManagerDashboard.userNonEvent];
	} else if ((window.location.hash === '#/' + FileManagerDashboard[FileManagerDashboard.events])) {
		currentPage = FileManagerDashboard[FileManagerDashboard.events];
	} else if ((window.location.hash === '#/' + FileManagerDashboard[FileManagerDashboard.accounts])) {
		currentPage = FileManagerDashboard[FileManagerDashboard.accounts];
	} else if ((window.location.hash === '#/' + FileManagerDashboard[FileManagerDashboard.cabangs])) {
		currentPage = FileManagerDashboard[FileManagerDashboard.cabangs];
	} else if ((window.location.hash === '#/' + FileManagerDashboard[FileManagerDashboard.filemanagers])) {
		currentPage = FileManagerDashboard[FileManagerDashboard.filemanagers];
	}
	return currentPage;
}

ej.base.rippleEffect(document.body, { selector: '.ripple-element', rippleFlag: true });
routeDefault();

var defaultSidebar = new ej.navigations.Sidebar({
	width: '200px',
	mediaQuery: window.matchMedia('(min-width: 768px)')
});
defaultSidebar.appendTo('#sidebar-indexdefault');

document.getElementById('sidebarRightpane').classList.add('sidebar-Rightpane');

var currentPage;
crossroads.addRoute('/:lang:', function () {
	var sample = currentPage || getCurrentPage();
	console.log(sample);
	if ((currentPage && currentPage !== '') || (window.location.hash === '#/' + getCurrentPage())) {
		if (!ej.base.isNullOrUndefined(document.querySelector('.expense-active-page'))) {
			document.querySelector('.expense-active-page').classList.remove('expense-active-page');
		}
		console.log(currentPage);
		var ajaxHTML = new ej.base.Ajax(currentPage.charAt(0).toUpperCase() + currentPage.slice(1), 'GET', true);
		ajaxHTML.send().then(function (value) {
			if (document.getElementById("range") && document.getElementById("range").ej2_instances) {
				rangeObj = document.getElementById("range").ej2_instances[0];
				rangeObj.destroy();
			}
			document.getElementById('content').innerHTML = '';
			document.getElementById('content').innerHTML = value.toString();

			renderControl("content");

			if ((currentPage === FileManagerDashboard[FileManagerDashboard.dashboard]) ||
				('#/' + FileManagerDashboard[FileManagerDashboard.dashboard] === window.location.hash)) {
				window.dashboard();
	
			} else if ((currentPage === FileManagerDashboard[FileManagerDashboard.userEvent]) ||
				('#/' + FileManagerDashboard[FileManagerDashboard.expense] === window.location.hash)) { }
			else if ((currentPage === FileManagerDashboard[FileManagerDashboard.userNonEvent]) ||
				('#/' + FileManagerDashboard[FileManagerDashboard.about] === window.location.hash)) {
			} else if ((currentPage === FileManagerDashboard[FileManagerDashboard.events]) ||
				('#/' + FileManagerDashboard[FileManagerDashboard.about] === window.location.hash)) {

			} else if ((currentPage === FileManagerDashboard[FileManagerDashboard.accounts]) ||
				('#/' + FileManagerDashboard[FileManagerDashboard.accounts] === window.location.hash)) {

			} else if ((currentPage === FileManagerDashboard[FileManagerDashboard.filemanagers]) ||
				('#/' + FileManagerDashboard[FileManagerDashboard.filemanagers] === window.location.hash)) {

			}
		});
	}
}).rules = { lang: ['index', 'dashboard', 'userEvent', 'userNonEvent', 'events','accounts','cabangs','filemanagers'] };
crossroads.bypassed.add(function (request) {
	var samplePath = ['index', 'dashboard', 'userEvent', 'userNonEvent', 'events','accounts','cabangs','filemanagers'];
	var hash = request.split(' ')[0];
	if (samplePath.indexOf(hash) === -1) {
		location.hash = '#/' + samplePath[0];
	}
});

function renderControl(id) {
	var scripts = document.querySelectorAll("#" + id + " script");
	var length = scripts.length;
	for (var i = 0; i < length; i++) {
		if (scripts[i].id == "")
			eval(scripts[i].text);
	}
}

for (var i = 0; i < document.querySelectorAll('li').length; i++) {
	document.querySelectorAll('li')[i].addEventListener('click', hash, false);
}
function hash(args) {
	document.getElementById('sidebar-wrapper').classList.remove('close');
	document.getElementById('overlay').classList.remove('dialog');
	document.getElementById('overlay').style.background = 'none';
	if (!ej.base.isNullOrUndefined(document.querySelector('.expense-active-page'))) {
		document.querySelector('.expense-active-page').classList.remove('expense-active-page');
	}
	(args.currentTarget).firstElementChild.classList.add('expense-active-page');
	hasher.setHash((args.currentTarget).firstElementChild.getAttribute('href').split('/')[1]);
}

function routeDefault() {
	crossroads.addRoute('', function () {
		window.location.href = window.location.href.lastIndexOf("/") === window.location.href.length - '/'.length ? '#/index' : window.location.href + "/#/index";
	});
}

hasher.initialized.add(function (hashValue) {
	crossroads.parse(hashValue);
});

hasher.changed.add(function (hashValue) {
	currentPage = hashValue;
	crossroads.parse(hashValue);
});
hasher.init();

window.onresize = function (args) {
	handleResize();
	if (!ej.base.Browser.isDevice) {
		if (document.getElementById('sidebar-wrapper') &&
			document.getElementById('sidebar-wrapper').classList.contains('open')) {
			document.getElementById('sidebar-wrapper').classList.remove('open');
		}
		if (document.getElementById('sidebar-wrapper') &&
			document.getElementById('sidebar-wrapper').classList.contains('close')) {
			document.getElementById('sidebar-wrapper').classList.remove('close');
		}
		if (document.getElementById('overlay') &&
			document.getElementById('overlay').classList.contains('dialog')) {
			document.getElementById('overlay').classList.remove('dialog');
		}
		if ((document.getElementsByClassName('filter')[0]) &&
			(document.getElementsByClassName('filter')[0]).classList.contains('filter-open')) {
			(document.getElementsByClassName('filter')[0]).classList.remove('filter-open');
		}
		if ((document.getElementsByClassName('filter')[0]) &&
			(document.getElementsByClassName('filter')[0]).classList.contains('filter-close')) {
			(document.getElementsByClassName('filter')[0]).classList.remove('filter-close');
		}
	}
};

document.getElementsByClassName('container-fluid rightpane')[0].addEventListener('scroll', document.onscroll);

document.onscroll = function (args) {
	if (document.getElementById("range") && document.getElementById("range").ej2_instances[0]) {
		var rangeSlider = document.getElementById("range").ej2_instances[0];
		rangeSlider.refreshTooltip();
	}
};

document.getElementById('menu-toggle').onclick = function () {
	menu = document.getElementById('sidebar-wrapper');
	overlay = document.getElementById('overlay');
	toggleMenu();
};
document.getElementById('filter-toggle').onclick = function () {
	toggleFilterMenu();
};
document.getElementById('overlay').onclick = function () {
	menu = document.getElementById('sidebar-wrapper');
	overlay = document.getElementById('overlay');
	if (document.getElementById("sidebar-indexexpense")) {
		var expenseFilterMenu = document.getElementById("sidebar-indexexpense").ej2_instances[0];
		expenseFilterMenu.hide();
	}
	defaultSidebar.hide();
	handleOverlay();
};
(document.getElementsByClassName('nav-list')[0]).onclick = function (args) {
	if ((args.target).nodeName === 'A') {
		menu = document.getElementById('sidebar-wrapper');
		overlay = document.getElementById('overlay');
		handleOverlay();
	}
};

function toggleMenu() {
	if (menu.classList.contains('open')) {
		removeToggleClass();
		defaultSidebar.hide();
		menu.classList.add('close');
		disableOverlay();
	} else if (menu.classList.contains('close')) {
		removeToggleClass();
		defaultSidebar.show();
		menu.classList.add('open');
		enableOverlay();
	} else {
		menu.classList.add('open');
		defaultSidebar.show();
		enableOverlay();
	}
}

function removeToggleClass() {
	menu.classList.remove('open');
	menu.classList.remove('close');
}

function enableOverlay() {
	overlay.classList.add('dialog');
	overlay.style.background = '#383838';
}

function disableOverlay() {
	overlay.classList.remove('dialog');
	overlay.style.background = 'none';
}

function toggleFilterMenu() {
	menu = document.getElementById('sidebar-wrapper');
	overlay = document.getElementById('overlay');
	menu.style.zIndex = '10000';
	var expenseFilterMenu = document.getElementById("sidebar-indexexpense").ej2_instances[0];
	var filterMenu = document.getElementsByClassName('sidebar-wrapper-filter')[0];
	if (filterMenu.classList.contains('filter-open')) {
		filterMenu.classList.remove('filter-open');
		filterMenu.classList.add('filter-close');
		expenseFilterMenu.hide();
		disableOverlay();
	} else if (filterMenu.classList.contains('filter-close')) {
		filterMenu.classList.remove('filter-close');
		filterMenu.classList.add('filter-open');
		expenseFilterMenu.show();
		enableOverlay();
	} else {
		filterMenu.classList.add('filter-open');
		expenseFilterMenu.show();
		enableOverlay();
	}
}

function handleOverlay() {
	disableOverlay();
	removeToggleClass();
	removeFilterToggleClass();
}

function removeFilterToggleClass() {
	menu.style.zIndex = '100001';
	var filterMenu = document.getElementsByClassName('sidebar-wrapper-filter')[0];
	if (!ej.base.isNullOrUndefined(filterMenu)) {
		filterMenu.classList.remove('filter-open');
		filterMenu.classList.remove('filter-close');
	}
}

function handleResize() {
	if (document.documentElement.offsetWidth > 1400) {
		document.body.style.minHeight = 'auto';
		document.body.style.minHeight = document.documentElement.offsetHeight + 'px';
	}
}