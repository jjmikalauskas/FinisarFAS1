   M S B A M L   `   `   `  ����F  BFinisarFAS1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:FinisarFAS1.ViewFinisarFAS1.View  5   	   ?$clr-namespace:FinisarFAS1.ConvertersFinisarFAS1.Converters  5   	   #    FinisarFAS1.View.DialogWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:FinisarFAS1.View     5   	   ;
converters$clr-namespace:FinisarFAS1.Converters     5   	     � WindowStartupLocation$  CenterScreen=�5      $	��340��5	   %   $	��650��62    � Title 6	   �� *	TitleText+ � 	Resources 5
       �� MergedDictionaries �� Source5   /FinisarFAS1.Converters.StringToVisibleConverter   NullVisibilityConverter@   :FinisarFAS1.Converters.StringToOppositeVisibilityConverter # OppositeNullVisibilityConverter6   0FinisarFAS1.Converters.BooleanToVisibleConverter  BooleanToVisibleConverter8   2FinisarFAS1.Converters.BooleanToCollapsedConverter  BooleanToCollapsedConverter@   :FinisarFAS1.Converters.OppositeBooleanToCollapsedConverter ' #OppositeBooleanToCollapsedConverter �� 
TargetType �� Property �� Value�� 5   
    5      �� 5      $ MainSkin.xaml?�6%   %U  &	        &	      &	 &     &	 9     &	 L     '�� _       5      62     5      6=     5      63     5      65     5      6=   �� 5      " ��6   .�5      ��  � ��$ 28(�6-   �� 5       � ��$ 25��6+   �� 5       � ��$	 120��6*   �� 5       � ��$ 0,2,0,0q�6+   5      5      .��5      �� $��3q�6   
���  ��._�5       ]� 
�������.+�5"   
   �� $��Bottom=�6    �� Orientation$ 
Horizontal=�6-   $��	12,0,12,8q�6F   $��Right=�6Y   .+�5#      �� 	 � ToolTip$	 Ok��6   
 �� 	IsDefault
 . $��	15,0,15,0q�63     
GreenGlass#��[ 5%      ��5$      �� *	OkCommand+��5&      �� *	YesText+y�5'      �� *	YesText+ �� 	Converter# [  �� 5*      $	 Cancel��6    �� IsCancel . $��	10,0,15,0q�66     RedGlass#��[ 5,      ��5+      �� *CancelCommand+��5-      �� *
CancelText+y�5.      �� *	YesText+# [  �� 50      $	 Cancel��6   
 . $��	10,0,15,0q�67   ��51      �� *CancelCommand+��52      �� *
CancelText+y�53      �� *	YesText+# [ 54      ]� 56   
   $	��Top=�6   $��0,0,0,10q�6)   .+�57      �� $��20(�6   $
��Wrap=�6*   $
��Auto��58      $
��Auto��6%    �� Padding$
 10,5q�63   $��Center=�59      $��Center=�65   ��5:      �� *	Message+ �� FallbackValue$ DIALOG MESSAGE��5;      5<      5=      5>      