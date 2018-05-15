Catel project template readme
=============================

Congratulations with creating a new Catel project:

LiveGeometry

To get this project up and running, perform the following actions:

1) Right-click on the project => Manage NuGet packages...
2) Search & install:
2.a) Catel.Core
2.b) Catel.MVVM
3) Build and run the application

Note that this project template assumes that you are using Catel 5.x.

Other interesting notes in this document;

1. Automatically transforming regular properties to Catel properties
2. ReSharper support
3. Orchestra & Orc components

For more information and support, visit http://www.catelproject.com



1. Automatically transforming regular properties to Catel properties
--------------------------------------------------------------------

To automatically transform a regular property into a Catel property, use
the following NuGet package:

* Catel.Fody

The following property definition:


    public string Name { get; set; }


will be weaved into:


    public string Name
    {
        get { return GetValue<string>(NameProperty); }
        set { SetValue(NameProperty, value); }
    }
     
    public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));

    
For more information, visit http://www.catelproject.com/tools/catel-fody/



2. ReSharper support
--------------------

Catel provides a ReSharper plugin to ease the development of Catel. 

For more information, visit http://www.catelproject.com/tools/catel-resharper/



3. Orchestra & Orc components
-----------------------------

If you are planning on using WPF, there is a huge set (60+) of free open-source components 
available based on Catel. All these open source are developed by a company called WildGums 
(see http://www.wildgums.com) and provided to the community for free. The components are well 
maintained and being used in several commercial WPF applications.

For more information, see https://github.com/wildgums