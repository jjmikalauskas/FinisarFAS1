   M S B A M L   `   `   `  ����F  BFinisarFAS1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:FinisarFAS1.ViewFinisarFAS1.View  5           FinisarFAS1.View.EmailView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:FinisarFAS1.View     5      $	��450��5      $	��800��6,     � Title$  Email��5       � 	Resources 5
       �� MergedDictionaries �� Source �� 
TargetType �� Property �� Value�� 5   
    5      �� 5      $ MainSkin.xaml?�6%   %�  '��       '�� �     �� 5      " ��6   .�5      ��  � ��$
 15,5q�6+   �� 5       � ��$ 25(�6-   �� 5       � ��$ 50��6+   �� 5       � ��$	 140��6*   5      �� 5      " ��6   .�5      ��  � ��$ 16(�6-   �� 5       � ��$ 25��6+   �� 5       � ��$	 120��6*   �� 5       � ��$	 0,5q�6+   5      5      .��5       � 
������� � ColumnDefinitions 5!   
   �� 5"      $��1* �6   �� 5#      $��1* �6   �� 5$      $��2* �6    � RowDefinitions 5&   
   �� 5'      $��3* �6   �� 5(      $��3* �6   �� 5)      $	��16* �6   �� 5*      $��4* �6   .+�5-   
   �� $��0��6   $��3��6"   $	��2,5q�66   	 �� Orientation$	 
Horizontal=�6C   .+�5.      �� $��90��6   $��18(�6   $��Send To:��6-   $��Center=�6@   �� 5/      $	��250��6   -  'System.Windows.Input.KeyboardNavigation
   TabIndex$
 0��6"   $��Center=�51      ��50      ��  �� Path$ SendTo� �� UpdateSourceTrigger$ 	LostFocus=�51      52      �� 54   
   $��1��6   $��3��6"   $	��2,5q�66   $	 
Horizontal=�6C   .+�55      �� $��90��6   $��18(�6   $��Subject:��6-   $��Center=�6@   �� 56      $	��250��6   $
 0��6"   $��Center=�58      ��57      �� $ Subject�$ 	LostFocus=�58      59      �� 5;   
   $��2��6   $��3��6   $��10,10q�63   $	��5,5q�6B   $
��Auto��5<      $
��Auto��6   $��Stretch=�5=      $��Stretch=�60    �� TextWrapping$ WrapWithOverflow=�5?      ��5>      �� *	EmailBody+5?      �� 5A   
   $��3��6   $��2��6"   $	 
Horizontal=�62   $��Right=�6K   .+�5F      �� -   5        $��	10,5,10,7q�5F      $	��160��6(    � ToolTip$ 
Send Email��64      	LimeGlass#��[  5G      $��
Send Email��5H       �� 	IsDefault . ��5J      �� $ SendEmailCmd�5K      5M      5N      